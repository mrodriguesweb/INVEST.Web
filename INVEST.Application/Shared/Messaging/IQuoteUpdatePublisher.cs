namespace INVEST.Application.Shared.Messaging
{
    public interface IQuoteUpdatePublisher
    {
        Task PublishAsync(QuoteUpdateRequestedMessage message, CancellationToken ct = default);
    }
}