using System;

namespace GestaoContratos.Domain.Entities
{
    public class UsuarioHistorico
    {
        public Guid Id { get; set; }
        public Guid UsuarioAlteradoId { get; set; }
        public Guid UsuarioQueAlterouId { get; set; }
        public string Acao { get; set; } = string.Empty;
        public DateTime DataAlteracao { get; set; }
    }
}
