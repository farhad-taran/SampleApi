using System.Threading.Tasks;
using Books.Api.Contracts.Common;
using Books.Api.Domain;
using CSharpFunctionalExtensions;

namespace Books.Api.Services
{
    public interface IBooksService
    {
        Task<Result<BookModel, Error>> GetBookAsync(string bookId);
        Task<Result<Unit, Error>> CreateBookAsync(BookModel model);
        Task<Result<Unit, Error>> UpdateBookAsync(string bookId, BookModel model);
        Task<Result<Unit, Error>> DeleteBookAsync(string bookId);
    }
}