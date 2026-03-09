using INVEST.Application.Acoes.Handlers;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Shared;
using INVEST.Infrastructure.Data;
using INVEST.Infrastructure.QuoteProviders;
using INVEST.Infrastructure.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((ctx, services) =>
    {

        services.AddScoped<IQuoteProvider, AlphaVantageQuoteProvider>();
        services.AddScoped<IPriceSnapshotRepository, PriceSnapshotRepository>();
        services.AddScoped<ProcessarQuoteUpdateHandler>();

        services.AddHttpClient<IQuoteProvider, AlphaVantageQuoteProvider>(client =>
        {
            client.BaseAddress = new Uri("https://www.alphavantage.co/");
        });

        services.AddDbContext<DataContext>(opts =>
            opts.UseNpgsql(ctx.Configuration.GetConnectionString("DefaultConnection")));
    })
    .Build();

host.Run();
