using Azure.Messaging.ServiceBus;
using invesT.Enums;
using INVEST.Application.Acoes.Queries;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Indicadores.Abstractions;
using INVEST.Application.Indicadores.Repository;
using INVEST.Application.QualidadeIndicador.Repository;
using INVEST.Application.Setores.Queries;
using INVEST.Application.Shared;
using INVEST.Application.Shared.Messaging;
using INVEST.Application.Shared.Services;
using INVEST.Application.TipoIndicador.Queries;
using INVEST.Application.TipoIndicador.Repository;
using INVEST.Domain.Entities;
using INVEST.Infrastructure.Data;
using INVEST.Infrastructure.Integrations.MarketData;
using INVEST.Infrastructure.Messaging;
using INVEST.Infrastructure.Notifications;
using INVEST.Infrastructure.Queries;
using INVEST.Infrastructure.QuoteProviders;
using INVEST.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Net;

namespace INVEST.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DB_INVEST")));

            services.AddHttpClient<IQuoteProvider, AlphaVantageQuoteProvider>(client =>
            {
                client.BaseAddress = new Uri("https://www.alphavantage.co/");
            });

            // Register MarketDataProviderClient - passes IConfiguration directly
            services.AddHttpClient<IIndicadoresMarketClient, MarketDataProviderClient>()
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer()
            })
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) +
                    TimeSpan.FromMilliseconds(new Random().Next(0, 1000))))
            .AddTransientHttpErrorPolicy(policyBuilder =>
                policyBuilder.CircuitBreakerAsync(
                    handledEventsAllowedBeforeBreaking: 5,
                    durationOfBreak: TimeSpan.FromMinutes(2)));

            var amqpUrl = config["RabbitMq:ConnectionUrl"];

            if (string.IsNullOrWhiteSpace(amqpUrl))
                throw new InvalidOperationException("A configuração 'RabbitMq:ConnectionUrl' não foi encontrada.");

            services.AddSingleton<IEventPublisher>(sp => new RabbitMqEventPublisher(amqpUrl));

            services.AddScoped<IAcaoQuery, AcaoQuery>();
            services.AddScoped<ISetorQuery, SetorQuery>();
            services.AddScoped<ITipoIndicadorQuery, TipoIndicadorQuery>();

            services.AddScoped<IAcaoRepository, AcaoRepository>();
            services.AddScoped<IPriceSnapshotRepository, PriceSnapshotRepository>();
            services.AddScoped<IIndicadorRepository, IndicadorRepository>();
            services.AddScoped<ITipoIndicadorRepository, TipoIndicadorRepository>();
            services.AddScoped<IQualidadeIndicadorRepository, QualidadeIndicadorRepository>();

            services.AddScoped<INotificationService, EmailNotificationService>();

            return services;
        }

        public static void ApplyMigrations(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.Migrate();
            }
        }

        public static void SeedDatabase(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DataContext>();

            if (!db.Setores.Any())
            {
                db.Setores.AddRange(
                    new Setor { Id = 1, Name = "ELÉTRICAS" },
                    new Setor { Id = 2, Name = "SEGUROS" },
                    new Setor { Id = 3, Name = "BANCO" },
                    new Setor { Id = 4, Name = "PETRÓLEO" },
                    new Setor { Id = 5, Name = "MINERAÇÃO" },
                    new Setor { Id = 6, Name = "ACESSÓRIOS" },
                    new Setor { Id = 7, Name = "ALIMENTOS" },
                    new Setor { Id = 8, Name = "BOLSA" },
                    new Setor { Id = 9, Name = "CONSTRUÇÃO" },
                    new Setor { Id = 10, Name = "FARMÁCIA" },
                    new Setor { Id = 11, Name = "IMÓVEIS" },
                    new Setor { Id = 12, Name = "INDÚSTRIA" }
                );
                db.SaveChanges();
            }

            if (!db.TiposIndicadores.Any())
            {
                db.TiposIndicadores.AddRange(
                    new TipoIndicador { Id = 1, Name = "Ebitda", Type = (byte)TypeIndicadorValue.DECIMAL },
                    new TipoIndicador { Id = 2, Name = "Dividend Yield", Type = (byte)TypeIndicadorValue.DECIMAL },
                    new TipoIndicador { Id = 3, Name = "ROE", Type = (byte)TypeIndicadorValue.DECIMAL },
                    new TipoIndicador { Id = 4, Name = "Margem Liquída", Type = (byte)TypeIndicadorValue.DECIMAL }
                );
                db.SaveChanges();

            }

            if (!db.NivelQualidade.Any())
            {
                db.NivelQualidade.AddRange(
                    new NivelQualidade { Id = 1, Nome = "Crítico" },
                    new NivelQualidade { Id = 2, Nome = "Alerta" },
                    new NivelQualidade { Id = 3, Nome = "Atenção" },
                    new NivelQualidade { Id = 4, Nome = "Bom" },
                    new NivelQualidade { Id = 5, Nome = "Excelente" }
                );
                db.SaveChanges();
            }

        }

    }
}