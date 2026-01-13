using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace INVEST.Infrastructure.Data
{
    public class DataContextFactory : IDesignTimeDbContextFactory<DataContext>
    {
        public DataContext CreateDbContext(string[] args)
        {
            // pega appsettings do projeto startup (Web) ou usa uma string fixa
            var basePath = Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

            var conn = configuration.GetConnectionString("DB_INVEST");

            var options = new DbContextOptionsBuilder<DataContext>()
                .UseNpgsql(conn)
                .Options;

            return new DataContext(options);
        }
    }
}
