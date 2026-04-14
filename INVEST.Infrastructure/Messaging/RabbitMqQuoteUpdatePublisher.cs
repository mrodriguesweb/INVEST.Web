using INVEST.Application.Shared.Messaging;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace INVEST.Infrastructure.Messaging
{
    public class RabbitMqQuoteUpdatePublisher : IQuoteUpdatePublisher, IAsyncDisposable
    {
        private readonly string _amqpUrl;
        private readonly string _exchange;
        private IConnection? _connection;

        public RabbitMqQuoteUpdatePublisher(string amqpUrl, string exchange)
        {
            _amqpUrl = amqpUrl;
            _exchange = exchange;
        }

        private async Task<IConnection> GetConnectionAsync(CancellationToken ct)
        {
            if (_connection is not null) return _connection;

            var factory = new ConnectionFactory { Uri = new Uri(_amqpUrl) };
            _connection = await factory.CreateConnectionAsync(ct);
            return _connection;
        }

        public async Task PublishAsync(QuoteUpdateRequestedMessage message, CancellationToken ct = default)
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var connection = await GetConnectionAsync(ct);

            await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

            await channel.ExchangeDeclareAsync(_exchange, ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: ct);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                MessageId = $"{message.Ticker.Id}-{message.RequestedAtUtc:yyyyMMddHHmmss}",
                Type = nameof(QuoteUpdateRequestedMessage)
            };

            await channel.BasicPublishAsync(
                exchange: _exchange,
                routingKey: string.Empty,
                mandatory: false,
                basicProperties: props,
                body: body,
                cancellationToken: ct);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection is not null)
            {
                try { await _connection.CloseAsync(); } catch { }
                try { await _connection.DisposeAsync(); } catch { }
            }
        }
    }
}