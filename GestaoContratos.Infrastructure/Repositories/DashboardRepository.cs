using System;
using System.Collections.Generic;
using Npgsql;
using GestaoContratos.Infrastructure.Database;

namespace GestaoContratos.Infrastructure.Repositories
{
    public class DashboardRepository
    {
        public (int total, int ativos, int vencidos, int aVencer) ObterResumo()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            const string sql = @"
                SELECT
                    COUNT(*) AS total,
                    COUNT(*) FILTER (WHERE fim_vigencia >= CURRENT_DATE) AS ativos,
                    COUNT(*) FILTER (WHERE fim_vigencia < CURRENT_DATE) AS vencidos,
                    COUNT(*) FILTER (
                        WHERE fim_vigencia >= CURRENT_DATE
                        AND fim_vigencia <= CURRENT_DATE + INTERVAL '30 days'
                    ) AS a_vencer
                FROM contratos;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();
            reader.Read();

            return (
                reader.GetInt32(0),
                reader.GetInt32(1),
                reader.GetInt32(2),
                reader.GetInt32(3)
            );
        }

        public List<(string numero, string contratada, DateTime fim, int dias)> ContratosAVencer()
        {
            var lista = new List<(string, string, DateTime, int)>();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            const string sql = @"
                SELECT
                    numero_contrato,
                    razao_social_contratada,
                    fim_vigencia,
                    (fim_vigencia - CURRENT_DATE) AS dias
                FROM contratos
                WHERE fim_vigencia >= CURRENT_DATE
                  AND fim_vigencia <= CURRENT_DATE + INTERVAL '15 days'
                ORDER BY fim_vigencia
                LIMIT 5;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add((
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetDateTime(2),
                    reader.GetInt32(3)
                ));
            }

            return lista;
        }

        public List<(string usuario, string campo, DateTime data)> HistoricoRecente()
        {
            var lista = new List<(string, string, DateTime)>();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            const string sql = @"
                SELECT usuario, campo, data_alteracao
                FROM contrato_historico
                ORDER BY data_alteracao DESC
                LIMIT 5;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add((
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetDateTime(2)
                ));
            }

            return lista;
        }
    }
}
