using System;
using System.Configuration;
using Npgsql;

namespace GestaoContratos.Infrastructure.Database
{
    public static class DatabaseConnection
    {
#if DEBUG
        private const string ConnectionName = "PostgresConnection_Dev";
#else
        private const string ConnectionName = "PostgresConnection_Prod";
#endif

        public static NpgsqlConnection GetConnection()
        {
            var connectionString =
                ConfigurationManager.ConnectionStrings[ConnectionName]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string não encontrada.");

            return new NpgsqlConnection(connectionString);
        }
    }
}
