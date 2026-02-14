using System;
using System.Collections.Generic;
using Npgsql;
using GestaoContratos.Domain.Entities;
using GestaoContratos.Domain.Enums;
using GestaoContratos.Domain.Interfaces;
using BCrypt.Net;
using GestaoContratos.Infrastructure.Database;

namespace GestaoContratos.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public bool LoginExiste(string login)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT COUNT(1) FROM usuarios WHERE login = @login";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);

            return (long)cmd.ExecuteScalar() > 0;
        }

        public string GerarLogin(string nomeCompleto)
        {
            var partes = nomeCompleto.ToLower().Split(' ');
            if (partes.Length < 2)
                return partes[0];

            return $"{partes[0]}.{partes[1]}";
        }

        public void Salvar(Usuario usuario)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            if (usuario.Id == Guid.Empty)
                usuario.Id = Guid.NewGuid();

            var sql = @"
                INSERT INTO usuarios
                (id, nomecompleto, login, email, senhahash, perfil, ativo, datacriacao)
                VALUES
                (@id, @nome, @login, @email, @senha, @perfil, @ativo, @data);
            ";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", usuario.Id);
            cmd.Parameters.AddWithValue("@nome", usuario.NomeCompleto);
            cmd.Parameters.AddWithValue("@login", usuario.Login);
            cmd.Parameters.AddWithValue("@email", usuario.Email);
            cmd.Parameters.AddWithValue("@senha", usuario.SenhaHash);
            cmd.Parameters.AddWithValue("@perfil", usuario.Perfil.ToString());
            cmd.Parameters.AddWithValue("@ativo", usuario.Ativo);
            cmd.Parameters.AddWithValue("@data", usuario.DataCriacao);

            cmd.ExecuteNonQuery();
        }

        public void Atualizar(Usuario usuario)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = @"
                UPDATE usuarios SET
                    nomecompleto = @nome,
                    email = @email,
                    perfil = @perfil,
                    ativo = @ativo
                WHERE id = @id;
            ";

            using var cmd = new NpgsqlCommand(sql, conn);

            cmd.Parameters.AddWithValue("@id", usuario.Id);
            cmd.Parameters.AddWithValue("@nome", usuario.NomeCompleto);
            cmd.Parameters.AddWithValue("@email", usuario.Email);
            cmd.Parameters.AddWithValue("@perfil", usuario.Perfil.ToString());
            cmd.Parameters.AddWithValue("@ativo", usuario.Ativo);

            cmd.ExecuteNonQuery();
        }

        public int ContarMastersAtivos()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT COUNT(1) FROM usuarios WHERE perfil = 'Master' AND ativo = true";

            using var cmd = new NpgsqlCommand(sql, conn);

            return (int)(long)cmd.ExecuteScalar();
        }

        public Usuario? ObterPorId(Guid id)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM usuarios WHERE id = @id";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return MapearUsuario(reader);
        }

        public Usuario? Autenticar(string login, string senha)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM usuarios WHERE login = @login LIMIT 1";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            var usuario = MapearUsuario(reader);

            if (!BCrypt.Net.BCrypt.Verify(senha, usuario.SenhaHash))
                return null;

            return usuario;
        }


        public Usuario? ObterPorLogin(string login)
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var sql = "SELECT * FROM usuarios WHERE login = @login";

            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@login", login);

            using var reader = cmd.ExecuteReader();

            if (!reader.Read())
                return null;

            return MapearUsuario(reader);
        }

        public List<Usuario> ObterTodos()
        {
            using var conn = DatabaseConnection.GetConnection();
            conn.Open();

            var lista = new List<Usuario>();

            var sql = "SELECT * FROM usuarios ORDER BY nomecompleto";

            using var cmd = new NpgsqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(MapearUsuario(reader));
            }

            return lista;
        }

        private Usuario MapearUsuario(NpgsqlDataReader reader)
        {
            return new Usuario
            {
                Id = reader.GetGuid(reader.GetOrdinal("id")),
                NomeCompleto = reader.GetString(reader.GetOrdinal("nome_completo")),
                Login = reader.GetString(reader.GetOrdinal("login")),
                Email = reader.GetString(reader.GetOrdinal("email")),
                SenhaHash = reader.GetString(reader.GetOrdinal("senha_hash")),
                Perfil = Enum.Parse<PerfilUsuario>(
                    reader.GetString(reader.GetOrdinal("perfil"))
                ),
                Ativo = reader.GetBoolean(reader.GetOrdinal("ativo")),
                DataCriacao = reader.GetDateTime(reader.GetOrdinal("data_criacao"))
            };
        }

    }
}
