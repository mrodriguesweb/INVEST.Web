namespace INVEST.Application.Shared.Messaging.QuoteUpdate
{
    public interface IQuoteUpdatePublisher
    {
        Task PublishAsync(QuoteUpdateRequestedMessage message, CancellationToken ct = default);
    }
}