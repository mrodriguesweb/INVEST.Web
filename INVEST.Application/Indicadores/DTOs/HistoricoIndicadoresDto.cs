namespace INVEST.Application.Indicadores.DTOs
{
    public record HistoricoIndicadoresDto
    {
        public decimal Ebitda { get; init; }

        public decimal Roe { get; init; }

        public decimal MargemLiquida { get; init; }

    }
}