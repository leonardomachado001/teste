using System;

namespace GestaoContratos.Domain.Filtros
{
    public class FiltroContrato
    {
        public string NumeroContrato { get; set; }
        public string Contratada { get; set; }
        public string Objeto { get; set; }
        public string GestorContrato { get; set; }

        /// <summary>
        /// true  = somente ativos
        /// false = somente vencidos
        /// null  = todos
        /// </summary>
        public bool? Ativo { get; set; }

      //  public DateTime? VigenciaDe { get; set; }
        //public DateTime? VigenciaAte { get; set; }
    }
}
