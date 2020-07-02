using Microsoft.AspNetCore.Http;
using Serilog;
using Serilog.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Books.Api.Domain;
using Books.Api.Extensions;

namespace Books.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        const string MessageTemplate = "HTTP {CorrelationId} {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionHandlingMiddleware(ILogger logger, RequestDelegate next)
        {
            _logger = logger?.ForContext<ExceptionHandlingMiddleware>() ?? throw new ArgumentNullException(nameof(logger));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (httpContext == null) throw new ArgumentNullException(nameof(httpContext));

            var start = Stopwatch.GetTimestamp();
            try
            {
                await _next(httpContext);
                var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

                var statusCode = httpContext.Response?.StatusCode;
                var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

                var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : _logger;
                log.Write(level, MessageTemplate, httpContext.GetCorrelationId(), httpContext.Request.Method, httpContext.Request.Path, statusCode,
                    elapsedMs);
            }
            catch (Exception ex)
            {
                LogException(httpContext, GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()), ex);
                await HandleExceptionAsync(httpContext);
            }
        }

        private Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var internalServerError = ErrorTypes.InternalServerError;
            var result = context.MapError(internalServerError.Code, internalServerError);
            return context.Response.WriteAsync(JsonConvert.SerializeObject(result.Value));
        }

        private void LogException(HttpContext httpContext, double elapsedMs, Exception ex)
        {
            LogForErrorContext(httpContext)
                .Error(ex, MessageTemplate, httpContext.GetCorrelationId(), httpContext.Request.Method, httpContext.Request.Path, 500, elapsedMs);
        }

        private ILogger LogForErrorContext(HttpContext httpContext)
        {
            var request = httpContext.Request;

            var result = _logger
                .ForContext("RequestHeaders", request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()), destructureObjects: true)
                .ForContext("RequestHost", request.Host)
                .ForContext("RequestProtocol", request.Protocol);

            if (request.HasFormContentType)
                result = result.ForContext("RequestForm", request.Form.ToDictionary(v => v.Key, v => v.Value.ToString()));

            return result;
        }

        private static double GetElapsedMilliseconds(long start, long stop) =>
            (stop - start) * 1000 / (double)Stopwatch.Frequency;
    }
}