using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Books.Api.Contracts.Common;
using Books.Api.Domain;

namespace Books.Api.Extensions
{
    /// <summary>
    /// Correlation Extensions for HttpContext
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// Gets the correlation identifier for the request
        /// </summary>
        /// <param name="httpContext">The HTTP Context</param>
        /// <returns>The Correlation-Id HTTP request header if present, otherwise <see cref="HttpContext.TraceIdentifier"/></returns>
        public static string GetCorrelationId(this HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("Correlation-Id", out var correlationId);
            return correlationId.FirstOrDefault() ?? httpContext.TraceIdentifier;
        }

        public static ObjectResult MapError(this HttpContext httpContext, string mainErrorCode, params Error[] errors)
        {
            var errorResponse = new ErrorResponse
            {
                ErrorCode = mainErrorCode,
                CorrelationId = httpContext.GetCorrelationId(),
                Errors = errors,
                TimeStamp = DateTimeOffset.Now,
                PathCalled = httpContext.GetPathCalled()
            };

            if (mainErrorCode == ErrorTypes.StorageItemNotFound)
            {
                return new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                };
            }

            if (mainErrorCode == ErrorTypes.StorageUnAvailableErrorCode)
            {
                return new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.ServiceUnavailable,
                };
            }

            if (mainErrorCode == ErrorTypes.UnhandledStorageErrorCode)
            {
                return new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.BadGateway,
                };
            }
            
            if (mainErrorCode == ErrorTypes.StorageItemConflictErrorCode)
            {
                return new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                };
            }

            if (mainErrorCode == ErrorTypes.ValidationErrorCode)
            {
                return new ObjectResult(errorResponse)
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                };
            }

            return new ObjectResult(errorResponse)
            {
                StatusCode = (int)HttpStatusCode.InternalServerError
            };
        }
        
        public static string GetPathCalled(this HttpContext context)
        {
            return context.Request.Method + ":" + context.Request.Host.ToUriComponent() + (context.Request.Path.HasValue ? context.Request.Path.Value : "");
        }
    }
}
