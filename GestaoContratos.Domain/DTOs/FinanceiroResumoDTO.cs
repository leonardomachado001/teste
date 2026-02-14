using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoContratos.Domain.DTOs
{
    public class FinanceiroResumoDTO
    {
        public decimal TotalContratado { get; set; }
        public decimal TotalAtivo { get; set; }
        public decimal TotalVencido { get; set; }
        public decimal TotalAVencer30Dias { get; set; }
    }
}
