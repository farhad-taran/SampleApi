using System.Data;

namespace Books.Api.Storage
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}