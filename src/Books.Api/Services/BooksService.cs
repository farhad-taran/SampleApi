using System.Threading.Tasks;
using Books.Api.Contracts.Common;
using Books.Api.Domain;
using Books.Api.Storage;
using Books.Api.Storage.Model;
using CSharpFunctionalExtensions;

namespace Books.Api.Services
{
    public class BooksService: IBooksService
    {
        private readonly IBooksRepository _booksRepository;

        public BooksService(IBooksRepository booksRepository)
        {
            _booksRepository = booksRepository;
        }
        public async Task<Result<BookModel, Error>> GetBookAsync(string bookId)
        {
            var result = await _booksRepository.LoadAsync(bookId);

            return result.IsFailure ? Result.Failure<BookModel, Error>(result.Error) : Result.Success<BookModel, Error>(new BookModel
            {
                AuthorId = result.Value.AuthorId,
                BookId = result.Value.BookId,
                Name = result.Value.Name
            });
        }

        public Task<Result<Unit, Error>> CreateBookAsync(BookModel model)
        {
            return _booksRepository.InsertAsync(new BookItem
            {
                BookId = model.BookId,
                AuthorId = model.AuthorId,
                Name = model.Name
            });
        }

        public Task<Result<Unit, Error>> UpdateBookAsync(string bookId, BookModel model)
        {
            return _booksRepository.UpdateAsync(new BookItem
            {
                BookId = bookId,
                AuthorId = model.AuthorId,
                Name = model.Name
            });
        }

        public Task<Result<Unit, Error>> DeleteBookAsync(string bookId)
        {
            return _booksRepository.DeleteAsync(bookId);
        }
    }
}
