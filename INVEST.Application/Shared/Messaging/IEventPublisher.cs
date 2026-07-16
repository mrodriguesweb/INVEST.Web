namespace INVEST.Application.Shared.Messaging
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T message, string exchange, string? messageId = null, CancellationToken ct = default) where T : class;
    }
}