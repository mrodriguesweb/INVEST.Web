using INVEST.Application.Acoes.Queries;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Setores.Queries;
using INVEST.Domain.Entities;
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
        }

    }
}
