using Books.Api.Contracts.Common;
using Books.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{

    [Route("api/")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult MapError(Error error)
        {
            return HttpContext.MapError(error.Code, error);
        }

        [NonAction]
        [ApiExplorerSettings(IgnoreApi = true)]
        public ObjectResult MapError(ErrorsAggregate errorsAggregate)
        {
            return HttpContext.MapError(errorsAggregate.MainErrorCode, errorsAggregate.Errors);
        }
    }
}