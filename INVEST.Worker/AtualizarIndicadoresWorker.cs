using INVEST.Application.Indicadores.Abstractions;
using INVEST.Application.Indicadores.Repository;
using INVEST.Application.Shared.Messaging;
using INVEST.Application.Shared.Messaging.QuoteUpdate;
using INVEST.Domain.Entities;
using INVEST.Domain.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace INVEST.Worker
{
    public class AtualizarIndicadoresWorker : BackgroundService
    {
        private readonly string _amqpUrl;
        private readonly ILogger<AtualizarIndicadoresWorker> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private IConnection? _connection;
        private IChannel? _channel;

        public AtualizarIndicadoresWorker(string amqpUrl, ILogger<AtualizarIndicadoresWorker> logger, IServiceScopeFactory scopeFactory)
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

            // --- 1. CONFIGURAÇÃO DA TOPOLOGIA DE DEAD LETTER (DLX / DLQ) ---
            // Declara a Exchange "Cemitério"
            await _channel.ExchangeDeclareAsync(MessagingConstants.Exchanges.DlqExchange, ExchangeType.Direct, durable: true, cancellationToken: stoppingToken);

            // Declara a Fila para onde as mensagens com erro fatal irão
            await _channel.QueueDeclareAsync(MessagingConstants.Queues.AtualizarIndicadoresDlq, durable: true, exclusive: false, autoDelete: false, cancellationToken: stoppingToken);

            // Liga a Fila à Exchange usando uma Routing Key específica
            await _channel.QueueBindAsync(MessagingConstants.Queues.AtualizarIndicadoresDlq, MessagingConstants.Exchanges.DlqExchange, "fatal_error_indicadores", cancellationToken: stoppingToken);

            // --- 2. CONFIGURAÇÃO DA TOPOLOGIA PRINCIPAL ---
            await _channel.ExchangeDeclareAsync(MessagingConstants.Exchanges.QuotesUpdated, ExchangeType.Fanout, durable: true, cancellationToken: stoppingToken);

            // Dicionário com as regras: "Se a mensagem morrer aqui, mande para a DlqExchange"
            var queueArguments = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", MessagingConstants.Exchanges.DlqExchange },
                { "x-dead-letter-routing-key", "fatal_error_indicadores" }
            };

            // Declara a fila principal INJETANDO as regras de DLQ
            await _channel.QueueDeclareAsync(
                queue: MessagingConstants.Queues.AtualizarIndicadores,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArguments,
                cancellationToken: stoppingToken);

            await _channel.QueueBindAsync(MessagingConstants.Queues.AtualizarIndicadores, MessagingConstants.Exchanges.QuotesUpdated, string.Empty, cancellationToken: stoppingToken);

            // REGRA DE OURO DO SCRAPING: Processar apenas 1 mensagem por vez
            await _channel.BasicQosAsync(0, 1, false, stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var ticker = "Desconhecido";
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var payload = JsonSerializer.Deserialize<QuoteUpdatedIntegrationEvent>(body);

                    if (payload != null)
                    {
                        ticker = payload.Ticker.Name;
                        _logger.LogInformation("Iniciando extração de indicadores para {Ticker}...", ticker);

                        // Cria o escopo para pegar o Client
                        using var scope = _scopeFactory.CreateScope();
                        var marketClient = scope.ServiceProvider.GetRequiredService<IIndicadoresMarketClient>();

                        // 1. Vai na internet e extrai os dados
                        var dto = await marketClient.ExtrairIndicadoresAsync(payload.Ticker.Name, stoppingToken);

                        // 2. Prepara a lista de entidades de domínio para salvar
                        var indicadoresParaSalvar = new List<Indicador>();

                        var tickerId = payload.Ticker.Id;

                        // 3. O DE-PARA: Transforma as propriedades do DTO em Entidades do Banco
                        if (dto.Ebitda > 0)
                            indicadoresParaSalvar.Add(new Indicador(tickerId, (int)TipoIndicadorEnum.Ebitda, dto.Ebitda));

                        if (dto.MargemLiquida > 0)
                            indicadoresParaSalvar.Add(new Indicador(tickerId, (int)TipoIndicadorEnum.MargemLiquida, dto.MargemLiquida));

                        if (dto.Roe > 0)
                            indicadoresParaSalvar.Add(new Indicador(tickerId, (int)TipoIndicadorEnum.Roe, dto.Roe));

                        // 4. Salva no Banco de Dados
                        var repository = scope.ServiceProvider.GetRequiredService<IIndicadorRepository>();
                        await repository.AddRange(indicadoresParaSalvar, stoppingToken);

                        _logger.LogInformation("Sucesso! Indicadores extraídos para {Ticker}", ticker);

                        // 3. O Jitter (Proteção anti-ban do Cloudflare)
                        var random = new Random();
                        var delay = random.Next(2500, 5000);
                        _logger.LogInformation("Aguardando {Delay}ms para esfriar o IP...", delay);
                        await Task.Delay(delay, stoppingToken);
                    }

                    // Só dá o ACK depois de ter salvo no banco e respeitado o Delay
                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao extrair indicadores para {Ticker}. Movendo para reprocessamento.", ticker);

                    // O RabbitMQ vê o false, checa as regras da fila e despacha a mensagem para a DLX!
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false, stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(MessagingConstants.Queues.AtualizarIndicadores, false, consumer, stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}