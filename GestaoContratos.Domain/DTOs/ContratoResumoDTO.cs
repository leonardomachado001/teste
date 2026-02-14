namespace GestaoContratos.Domain.DTOs
{
    public class ContratoResumoDTO
    {
        public int Total { get; set; }
        public int Ativos { get; set; }
        public int Vencidos { get; set; }
        public int AVencer30Dias { get; set; }
    }
}
