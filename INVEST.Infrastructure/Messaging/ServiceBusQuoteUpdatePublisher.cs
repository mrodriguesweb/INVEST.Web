using Azure.Messaging.ServiceBus;
using INVEST.Application.Shared.Messaging;
using INVEST.Application.Shared.Messaging.QuoteUpdate;
using System.Text.Json;

namespace INVEST.Infrastructure.Messaging
{
    public class ServiceBusQuoteUpdatePublisher(ServiceBusClient client) : IQuoteUpdatePublisher
    {
        private const string QueueName = "quotes-update";

        public async Task PublishAsync(QuoteUpdateRequestedMessage message, CancellationToken ct = default)
        {
            await using var sender = client.CreateSender(QueueName);

            var json = JsonSerializer.Serialize(message);

            var sbMessage = new ServiceBusMessage(json)
            {
                MessageId = $"{message.Ticker.Id}-{message.RequestedAtUtc:yyyyMMddHHmmss}",
                ContentType = "application/json",
                Subject = nameof(QuoteUpdateRequestedMessage)
            };

            await sender.SendMessageAsync(sbMessage, ct);
        }
    }
}
