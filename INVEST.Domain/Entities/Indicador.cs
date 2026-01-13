namespace INVEST.Domain.Entities
{
    public class Indicador
    {

        public int Id { get; set; }

        public int TickerId { get; set; }

        public int TipoIndicadorId { get; set; }

        public decimal? ValorDecimal { get; set; }

        public bool? ValorBool { get; set; }

        public short? ValorShort { get; set; }

        public DateOnly DataRegistro { get; set; }

        public TipoIndicador TipoIndicador { get; set; }

        public Ticker Ticker { get; set; }

    }
}