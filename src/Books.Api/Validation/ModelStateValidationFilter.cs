using System.Collections.Generic;
using System.Linq;
using Books.Api.Contracts.Common;
using Books.Api.Domain;
using Books.Api.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Books.Api.Validation
{
    public class ModelStateValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid) return;

            var validationErrors = context.ModelState.Where(kvp => kvp.Value.ValidationState == ModelValidationState.Invalid).SelectMany(Map).ToArray();

            context.Result = context.HttpContext.MapError(ErrorTypes.ValidationErrorCode, validationErrors);
        }

        private static IEnumerable<Error> Map(KeyValuePair<string, ModelStateEntry> mseKvp)
        {
            return mseKvp.Value.Errors.Select(er => new Error(mseKvp.Key, string.IsNullOrWhiteSpace(er.ErrorMessage) ? er.Exception.Message : er.ErrorMessage));
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}