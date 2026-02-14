using System;
using System.Collections.Generic;
using System.Text;

namespace GestaoContratos.Domain.DTOs
{
    public class RelatorioGestorDTO
    {
        public string Gestor { get; set; } = "";
        public int TotalContratos { get; set; }
        public int Ativos { get; set; }
        public int Vencidos { get; set; }
    }
}

