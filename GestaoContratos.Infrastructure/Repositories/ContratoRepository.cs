using System;
using System.Collections.Generic;
using Npgsql;
using GestaoContratos.Domain.Entities;
using GestaoContratos.Domain.DTOs;
using GestaoContratos.Domain.Filtros;
using GestaoContratos.Domain.Interfaces;
using GestaoContratos.Infrastructure.Database;

namespace GestaoContratos.Infrastructure.Repositories
{
    public class ContratoRepository : IContratoRepository
    {
        public void Salvar(Contrato contrato)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            if (contrato.Id == Guid.Empty)
                contrato.Id = Guid.NewGuid();

            var sql = @"
                INSERT INTO contratos 
                (id, numerocontrato, empresa, datainicio, datafim, valor)
                VALUES 
                (@id, @numero, @empresa, @inicio, @fim, @valor)
                ON CONFLICT (id)
                DO UPDATE SET
                    numerocontrato = @numero,
                    empresa = @empresa,
                    datainicio = @inicio,
                    datafim = @fim,
                    valor = @valor;";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", contrato.Id);
            cmd.Parameters.AddWithValue("numero", contrato.NumeroContrato);
            cmd.Parameters.AddWithValue("empresa", contrato.Empresa);
            cmd.Parameters.AddWithValue("inicio", contrato.DataInicio);
            cmd.Parameters.AddWithValue("fim", contrato.DataFim);
            cmd.Parameters.AddWithValue("valor", contrato.Valor);

            cmd.ExecuteNonQuery();
        }

        public bool ExisteNumeroContrato(string numeroContrato)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT COUNT(1) FROM contratos WHERE numerocontrato = @numero";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("numero", numeroContrato);

            var count = (long)cmd.ExecuteScalar();
            return count > 0;
        }

        public List<Contrato> Filtrar(FiltroContrato filtro)
        {
            var lista = new List<Contrato>();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM contratos";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new Contrato
                {
                    Id = reader.GetGuid(reader.GetOrdinal("id")),
                    NumeroContrato = reader.GetString(reader.GetOrdinal("numerocontrato")),
                    Empresa = reader.GetString(reader.GetOrdinal("empresa")),
                    DataInicio = reader.GetDateTime(reader.GetOrdinal("datainicio")),
                    DataFim = reader.GetDateTime(reader.GetOrdinal("datafim")),
                    Valor = reader.GetDecimal(reader.GetOrdinal("valor"))
                });
            }

            return lista;
        }

        public Contrato? ObterPorId(Guid id)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM contratos WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return new Contrato
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                NumeroContrato = reader.GetString(reader.GetOrdinal("numerocontrato")),
                Empresa = reader.GetString(reader.GetOrdinal("empresa")),
                DataInicio = reader.GetDateTime(reader.GetOrdinal("datainicio")),
                DataFim = reader.GetDateTime(reader.GetOrdinal("datafim")),
                Valor = reader.GetDecimal(reader.GetOrdinal("valor"))
            };
        }

        public void Excluir(Guid id)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "DELETE FROM contratos WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("id", id);
            cmd.ExecuteNonQuery();
        }

        public ContratoResumoDTO ObterResumo()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var resumo = new ContratoResumoDTO();

            var sql = @"
                SELECT COUNT(*) AS total,
                       COALESCE(SUM(valor),0) AS valortotal
                FROM contratos;";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                resumo.TotalContratos = reader.GetInt32(0);
                resumo.ValorTotal = reader.GetDecimal(1);
            }

            return resumo;
        }
    }
}
