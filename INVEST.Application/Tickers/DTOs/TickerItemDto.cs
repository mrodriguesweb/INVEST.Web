namespace INVEST.Application.Tickers.DTOs
{
    public sealed class TickerItemDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public decimal? LastQuote { get; set; }

    }
}