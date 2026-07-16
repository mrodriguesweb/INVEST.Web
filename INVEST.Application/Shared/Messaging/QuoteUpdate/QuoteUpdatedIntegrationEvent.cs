using INVEST.Application.Tickers.DTOs;

namespace INVEST.Application.Shared.Messaging.QuoteUpdate
{
    public record QuoteUpdatedIntegrationEvent(TickerUpdateQuoteDto Ticker, decimal NewPrice, DateTime UpdatedAtUtc);
}
