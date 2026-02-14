using System;
using Npgsql;
using GestaoContratos.Domain.Entities;
using GestaoContratos.Domain.Enums;
using GestaoContratos.Infrastructure.Database;
using GestaoContratos.Domain.Security;

namespace GestaoContratos.Infrastructure.Repositories
{
    public class UsuarioRepository
    {
        public Usuario? Autenticar(string login, string senha)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = @"
                SELECT 
                    id,
                    nomecompleto,
                    login,
                    email,
                    senhahash,
                    perfil,
                    ativo,
                    datacriacao
                FROM usuarios
                WHERE login = @login
                LIMIT 1;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            var senhaHashBanco = reader.GetString(reader.GetOrdinal("senhahash"));

            // 🔐 Valida senha usando seu PasswordHasher
            if (!PasswordHasher.VerificarSenha(senha, senhaHashBanco))
                return null;

            return new Usuario
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                NomeCompleto = reader.GetString(reader.GetOrdinal("nomecompleto")),
                Login = reader.GetString(reader.GetOrdinal("login")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                SenhaHash = senhaHashBanco,
                Perfil = Enum.Parse<PerfilUsuario>(
                    reader.GetString(reader.GetOrdinal("perfil"))
                ),
                Ativo = reader.GetBoolean(reader.GetOrdinal("ativo")),
                DataCriacao = reader.GetDateTime(reader.GetOrdinal("datacriacao"))
            };
        }
    }
}
