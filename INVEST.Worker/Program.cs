using INVEST.Application;
using INVEST.Infrastructure;
using INVEST.Worker;

var builder = Host.CreateApplicationBuilder(args);

// 1. Injeta as camadas da Clean Architecture
// (Garante que seu DbContext, Handlers e Publishers estejam disponíveis)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Lê a configuração do RabbitMQ
var amqpUrl = builder.Configuration["RabbitMq:ConnectionUrl"]
    ?? throw new InvalidOperationException("A configuração 'RabbitMq:ConnectionUrl' não foi encontrada.");

// 3. Registra o seu Worker Service no contêiner
builder.Services.AddHostedService(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ProcessarQuoteUpdateWorker>>();
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();

    return new ProcessarQuoteUpdateWorker(amqpUrl, logger, scopeFactory);
});

var host = builder.Build();
host.Run();