using INVEST.Application.Shared.Messaging;
using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace INVEST.Infrastructure.Messaging
{
    public class RabbitMqEventPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly string _amqpUrl;
        private IConnection? _connection;

        public RabbitMqEventPublisher(string amqpUrl)
        {
            _amqpUrl = amqpUrl;
        }

        private async Task<IConnection> GetConnectionAsync(CancellationToken ct)
        {
            if (_connection is not null) return _connection;

            var factory = new ConnectionFactory { Uri = new Uri(_amqpUrl) };
            _connection = await factory.CreateConnectionAsync(ct);
            return _connection;
        }

        public async Task PublishAsync<T>(T message, string exchange, string? messageId = null, CancellationToken ct = default) where T : class
        {
            if (message is null) throw new ArgumentNullException(nameof(message));

            var connection = await GetConnectionAsync(ct);
            await using var channel = await connection.CreateChannelAsync(cancellationToken: ct);

            // Declara o exchange dinamicamente baseado no parâmetro passado
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: ct);

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var props = new BasicProperties
            {
                ContentType = "application/json",
                DeliveryMode = DeliveryModes.Persistent,
                // Se um ID não for fornecido, gera um Guid único automaticamente
                MessageId = messageId ?? Guid.NewGuid().ToString(),
                // Pega o nome exato da classe que está sendo enviada
                Type = typeof(T).Name
            };

            await channel.BasicPublishAsync(
                exchange: exchange,
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