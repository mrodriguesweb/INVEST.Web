using INVEST.Application.Acoes.Handlers;
using INVEST.Application.Shared.Messaging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace INVEST.Worker
{
    public class ProcessarQuoteUpdateWorker : BackgroundService
    {
        private readonly string _amqpUrl;
        private readonly ILogger<ProcessarQuoteUpdateWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        private IConnection? _connection;
        private IChannel? _channel;

        private const string QueueName = "quotes-update";
        private const string ExchangeName = "quotes.exchange";
        private const string DlqExchangeName = "quotes.exchange.dlx";

        public ProcessarQuoteUpdateWorker(
            string amqpUrl,
            ILogger<ProcessarQuoteUpdateWorker> logger,
            IServiceScopeFactory scopeFactory)
        {
            _amqpUrl = amqpUrl;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(_amqpUrl)
            };

            _connection = await factory.CreateConnectionAsync(cancellationToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

            // 1. Configura a topologia: Fila Principal e Dead Letter Queue (DLQ)
            await SetupTopologyAsync(cancellationToken);

            await base.StartAsync(cancellationToken);
        }

        private async Task SetupTopologyAsync(CancellationToken ct)
        {
            // Declara o DLX e a DLQ
            await _channel!.ExchangeDeclareAsync(DlqExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: ct);
            await _channel.QueueDeclareAsync($"{QueueName}-dlq", durable: true, exclusive: false, autoDelete: false, cancellationToken: ct);
            await _channel.QueueBindAsync($"{QueueName}-dlq", DlqExchangeName, string.Empty, cancellationToken: ct);

            // Declara a Fila Principal apontando as rejeições para o DLX
            var queueArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", DlqExchangeName }
            };

            await _channel.ExchangeDeclareAsync(ExchangeName, ExchangeType.Fanout, durable: true, autoDelete: false, cancellationToken: ct);
            await _channel.QueueDeclareAsync(QueueName, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs, cancellationToken: ct);
            await _channel.QueueBindAsync(QueueName, ExchangeName, string.Empty, cancellationToken: ct);

            // Define o prefetch (quantas mensagens processar por vez simultaneamente)
            await _channel.BasicQosAsync(prefetchSize: 0, prefetchCount: 1, global: false, cancellationToken: ct);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel!);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var messageId = ea.BasicProperties.MessageId ?? "Desconhecido";
                var isRedelivered = ea.Redelivered; // Equivalente simplificado ao DeliveryCount > 1

                _logger.LogInformation("Processando mensagem {MessageId} | Redelivered: {Redelivered}", messageId, isRedelivered);

                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var payload = JsonSerializer.Deserialize<QuoteUpdateRequestedMessage>(body);

                    if (payload is null)
                    {
                        _logger.LogError("Payload inválido. Enviando para DLQ.");
                        // Requeue = false + Fila com x-dead-letter-exchange = Vai para a DLQ
                        await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                        return;
                    }

                    _logger.LogInformation("Payload Enviado: {payload}", payload);

                    // 2. Cria um escopo isolado para injetar os serviços (como DbContext)
                    using (var scope = _scopeFactory.CreateScope())
                    {
                        var handler = scope.ServiceProvider.GetRequiredService<ProcessarQuoteUpdateHandler>();

                        await handler.HandleAsync(payload, stoppingToken);

                        // O PRÓXIMO EVENTO ENTRA AQUI:
                        // var publisher = scope.ServiceProvider.GetRequiredService<IQuoteUpdatedPublisher>();
                        // await publisher.PublishAsync(new QuoteUpdatedIntegrationEvent(...));
                    }

                    // Sucesso = Ack
                    await _channel!.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                    _logger.LogInformation("Cotação processada e novo evento disparado com sucesso.");
                }
                catch (Exception ex)
                {
                    // Se for um erro de negócio conhecido, joga direto pra DLQ
                    if (ex is InvalidOperationException)
                    {
                        _logger.LogError(ex, "Erro de negócio ao processar {MessageId}. Movendo para DLQ.", messageId);
                        await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, cancellationToken: stoppingToken);
                    }
                    else
                    {
                        _logger.LogWarning(ex, "Falha de infraestrutura. Tentando novamente...");
                        await _channel!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                    }
                }
            };

            await _channel!.BasicConsumeAsync(QueueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

            // Mantém o worker rodando até o cancelamento
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_channel is not null) await _channel.CloseAsync(cancellationToken);
            if (_connection is not null) await _connection.CloseAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}