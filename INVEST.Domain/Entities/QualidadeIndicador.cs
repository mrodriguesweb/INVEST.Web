namespace INVEST.Domain.Entities
{
    public class QualidadeIndicador
    {

        public int Id { get; set; }

        public int TipoIndicadorId { get; set; }

        public int NivelQualidadeId { get; set; }

        public decimal ValorMinimo { get; set; }

        public decimal ValorMaximo { get; set; }

        public NivelQualidade NivelQualidade { get; set; }

        public TipoIndicador TipoIndicador { get; set; }

    }
}