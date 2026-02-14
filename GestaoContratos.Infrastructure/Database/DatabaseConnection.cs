using Npgsql;

namespace GestaoContratos.Infrastructure.Database
{
    public static class DatabaseConnection
    {
        private static string? _connectionString;

        public static void Configure(string connectionString)
        {
            _connectionString = connectionString;
        }

        public static NpgsqlConnection GetConnection()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Connection string não configurada.");

            return new NpgsqlConnection(_connectionString);
        }
    }
}
