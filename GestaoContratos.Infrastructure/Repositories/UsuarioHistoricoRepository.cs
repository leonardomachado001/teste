using GestaoContratos.Domain;
using GestaoContratos.Domain.Entities;
using GestaoContratos.Infrastructure.Database;
using Npgsql;
using System;
using System.Collections.Generic;

namespace GestaoContratos.Infrastructure.Repositories
{
    public class UsuarioHistoricoRepository
    {
        public void Registrar(Guid usuarioAlteradoId, string acao)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            const string sql = @"
                INSERT INTO usuario_historico
                (id, usuario_alterado_id, usuario_que_alterou_id, acao, data_alteracao)
                VALUES (@id, @alterado, @alterou, @acao, @data)";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", Guid.NewGuid());
            cmd.Parameters.AddWithValue("@alterado", usuarioAlteradoId);
            cmd.Parameters.AddWithValue("@alterou", SessaoUsuario.UsuarioId);
            cmd.Parameters.AddWithValue("@acao", acao);
            cmd.Parameters.AddWithValue("@data", DateTime.Now);

            cmd.ExecuteNonQuery();
        }

        public List<dynamic> Listar()
        {
            var lista = new List<dynamic>();

            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            const string sql = @"
        SELECT 
            h.id,
            u1.nome_completo AS usuario_alterado,
            u2.nome_completo AS alterado_por,
            h.acao,
            h.data_alteracao
        FROM usuario_historico h
        JOIN usuarios u1 ON u1.id = h.usuario_alterado_id
        JOIN usuarios u2 ON u2.id = h.usuario_que_alterou_id
        ORDER BY h.data_alteracao DESC";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new
                {
                    UsuarioAlterado = reader["usuario_alterado"].ToString(),
                    AlteradoPor = reader["alterado_por"].ToString(),
                    Acao = reader["acao"].ToString()?.Replace(" | ", Environment.NewLine),
                    Data = Convert.ToDateTime(reader["data_alteracao"])
                        .ToString("dd/MM/yyyy HH:mm")
                });
            }

            return lista;
        }

    }
}
