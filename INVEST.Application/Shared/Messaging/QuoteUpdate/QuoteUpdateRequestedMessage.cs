using INVEST.Application.Tickers.DTOs;

namespace INVEST.Application.Shared.Messaging
{
    public record QuoteUpdateRequestedMessage(TickerUpdateQuoteDto Ticker, DateTime RequestedAtUtc);
}