using System;

namespace GestaoContratos.Domain.Entities
{
    public class ContratoValorExercicio
    {
        public Guid Id { get; set; }

        public int Ano { get; set; }
        public decimal Valor { get; set; }
    }
}
