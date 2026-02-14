using GestaoContratos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GestaoContratos.Domain.Security
{
    public static class PerfilPermissoes
    {
        private static readonly Dictionary<PerfilUsuario, Permissao[]> _mapa =
            new()
            {
                [PerfilUsuario.Master] =
                    Enum.GetValues(typeof(Permissao)).Cast<Permissao>().ToArray(),

                [PerfilUsuario.Gestor] = new[]
                {
                    Permissao.Dashboard,
                    Permissao.Usuario_Listar,
                    Permissao.Contrato_Listar,
                    Permissao.Contrato_Detalhar,
                    Permissao.Contrato_Criar,
                    Permissao.Contrato_Editar,
                    Permissao.Relatorio_Visualizar
                },

                [PerfilUsuario.Usuario] = new[]
                {
                    Permissao.Dashboard,
                    Permissao.Contrato_Listar,
                    Permissao.Contrato_Detalhar,
                    Permissao.Contrato_Criar,
                    Permissao.Contrato_Editar,
                    Permissao.Relatorio_Visualizar
                },

                [PerfilUsuario.Leitor] = new[]
                {
                    Permissao.Dashboard,
                    Permissao.Contrato_Listar,
                    Permissao.Contrato_Detalhar,
                    Permissao.Relatorio_Visualizar
                }
            };

        // =====================================
        // 🔐 Permissão simples por enum
        // =====================================
        public static bool TemPermissao(
            PerfilUsuario perfil,
            Permissao permissao)
        {
            return _mapa.ContainsKey(perfil) &&
                   _mapa[perfil].Contains(permissao);
        }

        // =====================================
        // 🔐 Pode ver usuário na lista?
        // =====================================
        public static bool PodeVerUsuario(
            PerfilUsuario perfilLogado,
            PerfilUsuario perfilUsuarioAlvo)
        {
            if (perfilLogado == PerfilUsuario.Master)
                return true;

            if (perfilLogado == PerfilUsuario.Gestor &&
                perfilUsuarioAlvo == PerfilUsuario.Master)
                return false;

            return true;
        }

        // =====================================
        // 🔐 Pode criar determinado perfil?
        // =====================================
        public static bool PodeCriarPerfil(
            PerfilUsuario perfilLogado,
            PerfilUsuario perfilNovo)
        {
            if (perfilLogado == PerfilUsuario.Master)
                return true;

            if (perfilLogado == PerfilUsuario.Gestor &&
                perfilNovo == PerfilUsuario.Master)
                return false;

            return true;
        }

        // =====================================
        // 🔐 Pode editar determinado perfil?
        // =====================================
        public static bool PodeEditarPerfil(
            PerfilUsuario perfilLogado,
            PerfilUsuario perfilUsuarioAlvo)
        {
            if (perfilLogado == PerfilUsuario.Master)
                return true;

            if (perfilLogado == PerfilUsuario.Gestor &&
                perfilUsuarioAlvo == PerfilUsuario.Master)
                return false;

            return true;
        }

        // =====================================
        // 🔐 Perfis visíveis no cadastro
        // =====================================
        public static IEnumerable<PerfilUsuario> PerfisPermitidosParaCriacao(
            PerfilUsuario perfilLogado)
        {
            if (perfilLogado == PerfilUsuario.Master)
                return Enum.GetValues(typeof(PerfilUsuario))
                           .Cast<PerfilUsuario>();

            if (perfilLogado == PerfilUsuario.Gestor)
                return new[]
                {
                    PerfilUsuario.Usuario,
                    PerfilUsuario.Leitor
                };

            return Enumerable.Empty<PerfilUsuario>();
        }
    }
}
