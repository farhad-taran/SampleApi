using System.Threading.Tasks;
using Books.Api.Contracts.Common;
using Books.Api.Services;
using Books.Api.Storage;
using Books.Api.Storage.Model;
using Microsoft.AspNetCore.Mvc;

namespace Books.Api.Controllers
{
    /// <summary>
    /// Simple CRUD controller
    ///
    /// All logic is tested in Acceptance Tests against actual Docker Images
    ///
    ///  
    /// All models are validated using fluent validation
    /// Avoiding throwing of unnecessary exceptions and handling known exceptions early
    /// in cases where exceptions are known a Result with a Error type is returned, this
    /// Result then is mapped to an appropriate HTTP response code
    /// 
    /// the benefits of using the Result Monad and mapping errors, are:
    /// 
    /// all common error types are in a single place
    /// all if and else statements are in a single place, no need to repeat in every controller
    /// True exceptions will stand out in logs
    /// Debugging is easier as the code will not jump to different areas due to exceptions
    /// </summary>
    public class BooksController : BaseApiController
    {
        private readonly IBooksService _booksService;

        public BooksController(IBooksService booksService)
        {
            _booksService = booksService;
        }

        [HttpPost(ApiRoutes.Books.CreateBook)]
        public async Task<IActionResult> CreateBookAsync(BookModel model)
        {
            var result = await _booksService.CreateBookAsync(model);

            return result.IsSuccess ? Ok(model) : MapError(result.Error);
        }

        [HttpGet(ApiRoutes.Books.GetBook)]
        public async Task<IActionResult> GetBookAsync(string bookId)
        {
            var result = await _booksService.GetBookAsync(bookId);

            return result.IsSuccess ? Ok(result.Value) : MapError(result.Error);
        }

        [HttpPut(ApiRoutes.Books.UpdateBook)]
        public async Task<IActionResult> UpdateBookAsync([FromRoute] string bookId, [FromBody] BookModel model)
        {
            var result = await _booksService.UpdateBookAsync(bookId, model);

            return result.IsSuccess ? Ok(result.Value) : MapError(result.Error);
        }

        [HttpDelete(ApiRoutes.Books.DeleteBook)]
        public async Task<IActionResult> DeleteBookAsync(string bookId)
        {
            var result = await _booksService.DeleteBookAsync(bookId);

            return result.IsSuccess ? Ok(result.Value) : MapError(result.Error);
        }
    }
}