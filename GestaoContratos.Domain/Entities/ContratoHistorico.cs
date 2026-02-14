using System;

namespace GestaoContratos.Domain.Entities
{
    public class ContratoHistorico
    {
        public Guid Id { get; set; }
        public Guid ContratoId { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public DateTime DataAlteracao { get; set; }
        public string Campo { get; set; } = string.Empty;
        public string? ValorAnterior { get; set; }
        public string? ValorNovo { get; set; }
    }
}
