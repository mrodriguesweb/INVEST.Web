using INVEST.Application.Shared.Messaging;
using INVEST.Application.Shared.Messaging.QuoteUpdate;
using INVEST.Application.Shared.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace INVEST.Worker
{
    public class NotificarQuoteUpdatedWorker : BackgroundService
    {
        private readonly string _amqpUrl;
        private readonly ILogger<NotificarQuoteUpdatedWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public NotificarQuoteUpdatedWorker(string amqpUrl, ILogger<NotificarQuoteUpdatedWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _amqpUrl = amqpUrl;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory { Uri = new Uri(_amqpUrl) };
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            // Configura a fila de notificações vinculada ao exchange de "atualizados"
            await _channel.ExchangeDeclareAsync(MessagingConstants.Exchanges.QuotesUpdated, ExchangeType.Fanout, durable: true, cancellationToken: stoppingToken);
            await _channel.QueueDeclareAsync(MessagingConstants.Queues.NotificarEmail, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);
            await _channel.QueueBindAsync(MessagingConstants.Queues.NotificarEmail, MessagingConstants.Exchanges.QuotesUpdated, string.Empty, cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var payload = JsonSerializer.Deserialize<QuoteUpdatedIntegrationEvent>(body);

                    if (payload != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

                        await notificationService.SendAsync(
                            title: $"Alerta de Cotação: {payload.Ticker.Name}",
                            message: $"A ação {payload.Ticker.Name} foi atualizada para {payload.NewPrice:C2} em {payload.UpdatedAtUtc:HH:mm:ss}.",
                            ct: stoppingToken);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar notificação.");
                    // Em caso de erro, aqui poderíamos mandar para uma DLQ específica de notificações
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, false, stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(MessagingConstants.Queues.NotificarEmail, false, consumer, stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}