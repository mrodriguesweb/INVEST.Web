namespace INVEST.Application.Indicadores.DTOs;

public record TickerIndicadoresDto
{
    public string Ticker { get; init; } = string.Empty;

    public HistoricoIndicadoresDto HistoricoIndicadores { get; init; } = new();

    public string Setor { get; init; } = string.Empty;
}