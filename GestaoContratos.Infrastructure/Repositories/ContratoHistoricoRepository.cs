using System;
using System.Collections.Generic;
using Npgsql;
using GestaoContratos.Domain.Entities;
using GestaoContratos.Domain.Interfaces;
using GestaoContratos.Infrastructure.Database;

namespace GestaoContratos.Infrastructure.Repositories
{
    public class ContratoHistoricoRepository : IContratoHistoricoRepository
    {
        public void RegistrarAlteracao(ContratoHistorico historico)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            const string sql = @"
                INSERT INTO contrato_historico (
                    id,
                    contrato_id,
                    usuario,
                    data_alteracao,
                    campo,
                    valor_anterior,
                    valor_novo
                )
                VALUES (
                    @id,
                    @contrato_id,
                    @usuario,
                    @data,
                    @campo,
                    @valor_anterior,
                    @valor_novo
                );
            ";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("@contrato_id", historico.ContratoId);
            cmd.Parameters.AddWithValue("@usuario", historico.Usuario);
            cmd.Parameters.AddWithValue("@data", historico.DataAlteracao);
            cmd.Parameters.AddWithValue("@campo", historico.Campo);
            cmd.Parameters.AddWithValue("@valor_anterior", historico.ValorAnterior ?? "");
            cmd.Parameters.AddWithValue("@valor_novo", historico.ValorNovo ?? "");

            cmd.ExecuteNonQuery();
        }

        public List<ContratoHistorico> ObterPorContrato(Guid contratoId)
        {
            var lista = new List<ContratoHistorico>();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            const string sql = @"
                SELECT
                    id,
                    usuario,
                    data_alteracao,
                    campo,
                    valor_anterior,
                    valor_novo
                FROM contrato_historico
                WHERE contrato_id = @contrato_id
                ORDER BY data_alteracao DESC;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@contrato_id", contratoId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new ContratoHistorico
                {
                    Id = reader.GetGuid(0),
                    Usuario = reader.GetString(1),
                    DataAlteracao = reader.GetDateTime(2),
                    Campo = reader.GetString(3),
                    ValorAnterior = reader.IsDBNull(4) ? null : reader.GetString(4),
                    ValorNovo = reader.IsDBNull(5) ? null : reader.GetString(5)
                });
            }

            return lista;
        }
    }
}
