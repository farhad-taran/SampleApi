using System.Threading.Tasks;
using Books.Api.Contracts.Common;
using Books.Api.Domain;
using Books.Api.Storage.Model;
using CSharpFunctionalExtensions;
using Dapper;
using MySql.Data.MySqlClient;
using Serilog;

namespace Books.Api.Storage
{
    public class BooksRepository : IBooksRepository
    {
        private readonly IDbConnectionFactory _dbConnectionFactory;
        private readonly ILogger _logger;

        public BooksRepository(IDbConnectionFactory dbConnectionFactory, ILogger logger)
        {
            _dbConnectionFactory = dbConnectionFactory;
            _logger = logger;
        }

        public async Task<Result<BookItem, Error>> LoadAsync(string bookId)
        {
            try
            {
                using var connection = _dbConnectionFactory.Create();
                var sql = $"SELECT * from books.book_items WHERE book_id = '{bookId}'";
                var bookItem = await connection.QuerySingleOrDefaultAsync<BookItem>(sql);

                return bookItem == null ? Result.Failure<BookItem, Error>(ErrorTypes.FailedLocatingItem(bookId)) : Result.Success<BookItem, Error>(bookItem);
            }
            catch (MySqlException mySqlException)
            {
                _logger.Fatal(mySqlException, "failed loading book: {BookId}", bookId);
                return Result.Failure<BookItem, Error>(ErrorTypes.FailedLoadingItem(bookId, mySqlException.Number));
            }
        }

        public async Task<Result<Unit, Error>> InsertAsync(BookItem model)
        {
            try
            {
                using var conn = _dbConnectionFactory.Create();

                const string sql =
                    @"INSERT INTO 
                        book_items 
                        (
                            book_id,
                            author_id, 
                            name
                        ) 
                        VALUES 
                        (
                            @BookId,
                            @AuthorId, 
                            @Name
                        );";

                await conn.ExecuteScalarAsync(sql, model);

                return Result.Success<Unit, Error>(Unit.Instance);
            }
            catch (MySqlException mySqlException)
            {
                _logger.Fatal(mySqlException, "failed inserting new book");
                return Result.Failure<Unit, Error>(ErrorTypes.FailedSavingItem(mySqlException.Message, mySqlException.Number));
            }
        }

        public async Task<Result<Unit, Error>> UpdateAsync(BookItem model)
        {
            try
            {
                using var conn = _dbConnectionFactory.Create();
                const string updateSql =
                    @"UPDATE 
                            book_items 
                        SET 
                            author_id = @AuthorId,
                            name = @Name
                        WHERE
                            book_id = @BookId";

                var updateResult = await conn.ExecuteAsync(updateSql, model);

                return updateResult == 0 ? Result.Failure<Unit, Error>(ErrorTypes.FailedLocatingItem(model.BookId)) : Result.Success<Unit, Error>(Unit.Instance);
            }
            catch (MySqlException mySqlException)
            {
                _logger.Fatal(mySqlException, "failed updating book: {BookId}", model?.BookId);
                return Result.Failure<Unit, Error>(ErrorTypes.FailedUpdatingItem(model?.BookId, mySqlException.Message, mySqlException.Number));
            }
        }

        public async Task<Result<Unit, Error>> DeleteAsync(string bookId)
        {
            try
            {
                using var conn = _dbConnectionFactory.Create();
                const string sql =
                    @"DELETE FROM 
                            book_items
                        WHERE
                            book_id = @BookId";

                var updateResult = await conn.ExecuteAsync(sql, new { BookId = bookId });

                return updateResult == 0 ? Result.Failure<Unit, Error>(ErrorTypes.FailedLocatingItem(bookId)) : Result.Success<Unit, Error>(Unit.Instance);
            }
            catch (MySqlException mySqlException)
            {
                _logger.Fatal(mySqlException, "failed deleting book: {BookId}", bookId);
                return Result.Failure<Unit, Error>(ErrorTypes.FailedDeletingItem(bookId, mySqlException.Message, mySqlException.Number));
            }
        }
    }
}