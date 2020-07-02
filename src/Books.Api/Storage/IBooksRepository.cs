using System;
using System.Threading.Tasks;
using Books.Api.Contracts.Common;
using Books.Api.Domain;
using Books.Api.Storage.Model;
using CSharpFunctionalExtensions;

namespace Books.Api.Storage
{
    public interface IBooksRepository
    {
        Task<Result<BookItem, Error>> LoadAsync(string bookId);
        Task<Result<Unit, Error>> InsertAsync(BookItem bookItem);
        Task<Result<Unit,Error>> UpdateAsync(BookItem bookItem);
        Task<Result<Unit,Error>> DeleteAsync(string bookId);
    }
}
