using GestaoContratos.Domain.Enums;
using System;

namespace GestaoContratos.Domain
{
    public static class SessaoUsuario
    {
        public static Guid UsuarioId { get; set; }
        public static string NomeUsuario { get; set; } = "Sistema";
        public static PerfilUsuario Perfil { get; set; }
    }
}
