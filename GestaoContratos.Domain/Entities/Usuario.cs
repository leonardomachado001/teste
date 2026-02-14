using GestaoContratos.Domain.Enums;
using System;

namespace GestaoContratos.Domain.Entities
{
    public class Usuario
    {
        public Guid Id { get; set; }

        public string NomeCompleto { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public string SenhaHash { get; set; } = string.Empty;

        public PerfilUsuario Perfil { get; set; }

        public bool Ativo { get; set; } = true;

        public DateTime DataCriacao { get; set; }
    }
}
