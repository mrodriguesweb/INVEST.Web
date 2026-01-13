using INVEST.Application.Acoes.Queries;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Setores.Queries;
using INVEST.Infrastructure.Data;
using INVEST.Infrastructure.Queries;
using INVEST.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace INVEST.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DB_INVEST")));

            services.AddScoped<IAcaoQuery, AcaoQuery>();
            services.AddScoped<ISetorQuery, SetorQuery>();
            services.AddScoped<IAcaoRepository, AcaoRepository>();

            return services;
        }
    }
}
