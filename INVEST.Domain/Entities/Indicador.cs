using System.Xml.Linq;

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

        private Indicador() { }

        // Construtor 1: Apenas Decimal
        public Indicador(int tickerId, int tipoIndicadorId, decimal valor)
        {
            TickerId = tickerId;
            TipoIndicadorId = tipoIndicadorId;
            ValorDecimal = valor;
        }

        // Construtor 2: Apenas Bool
        public Indicador(int tickerId, int tipoIndicadorId, bool valor)
        {
            TickerId = tickerId;
            TipoIndicadorId = tipoIndicadorId;
            ValorBool = valor;
        }

        // Construtor 3: Apenas Short
        public Indicador(int tickerId, int tipoIndicadorId, short valor)
        {
            TickerId = tickerId;
            TipoIndicadorId = tipoIndicadorId;
            ValorShort = valor;
        }

    }
}