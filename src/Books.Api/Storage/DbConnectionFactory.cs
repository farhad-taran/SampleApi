using System.Data;
using Books.Api.Configuration;
using Books.Api.Domain;
using Dapper;
using MySql.Data.MySqlClient;

namespace Books.Api.Storage
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly StorageSettings _storageSettings;

        public DbConnectionFactory(StorageSettings storageSettings)
        {
            _storageSettings = storageSettings;
            DefaultTypeMap.MatchNamesWithUnderscores = true;
        }

        public IDbConnection Create()
        {
            return new MySqlConnection(_storageSettings.ConnectionString);
        }
    }
}