using System;
using System.Configuration;
using Npgsql;

namespace GestaoContratos.Infrastructure.Database
{
    public static class DatabaseConnection
    {
        public static NpgsqlConnection GetConnection()
        {
            var connectionString =
                ConfigurationManager.ConnectionStrings["PostgresConnection"]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Connection string não encontrada no App.config.");

            return new NpgsqlConnection(connectionString);
        }
    }
}
