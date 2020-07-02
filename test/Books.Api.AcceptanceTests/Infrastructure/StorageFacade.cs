using System.Threading.Tasks;
using Books.Api.Contracts.Common;
using Books.Api.Domain;
using Books.Api.Storage;
using Books.Api.Storage.Model;
using CSharpFunctionalExtensions;
using Dapper;

namespace Books.Api.AcceptanceTests.Infrastructure
{
    public class StorageFacade
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly IBooksRepository _booksRepository;

        public StorageFacade(IDbConnectionFactory dbConnectionFactory, IBooksRepository booksRepository)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _booksRepository = booksRepository;
        }

        public Task<Result<BookItem, Error>> LoadBookAsync(string bookId)
        {
            return _booksRepository.LoadAsync(bookId);
        }

        public void CleanUp()
        {
            using var conn = _dbConnectionFactory.Create();
            conn.Execute("DELETE FROM book_items WHERE book_id != 'healthcheck'");
        }

        public Task<Result<Unit, Error>> InsertBookAsync(BookItem book)
        {
            return _booksRepository.InsertAsync(book);
        }
    }
}
