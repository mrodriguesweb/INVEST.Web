using INVEST.Application.Acoes.Handlers;
using INVEST.Application.Acoes.Services;
using INVEST.Application.Setores.Services;
using Microsoft.Extensions.DependencyInjection;

namespace INVEST.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IAcaoService, AcaoService>();
            services.AddScoped<ISetorService, SetorService>();
            services.AddScoped<CreateAcaoHandler>();
            services.AddScoped<EditAcaoHandler>();
            services.AddScoped<DeleteAcaoHandler>();
            services.AddScoped<AtualizarCotacoesHandler>();

            return services;
        }
    }
}