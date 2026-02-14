using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoContratos.Domain.DTOs
{
    public class FinanceiroAgrupadoDTO
    {
        public string Nome { get; set; } = "";
        public decimal Total { get; set; }
        public decimal Ativo { get; set; }
        public decimal Vencido { get; set; }
    }
}
