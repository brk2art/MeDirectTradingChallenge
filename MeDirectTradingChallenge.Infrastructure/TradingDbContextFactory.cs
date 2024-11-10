using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MeDirectTradingChallenge.Infrastructure
{
    public class TradingDbContextFactory : IDesignTimeDbContextFactory<TradingDbContext>
    {
        public TradingDbContext CreateDbContext(string[] args)
        {
            // Create configuration
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            // Get the connection string from the configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Set up DbContextOptions with the connection string
            var optionsBuilder = new DbContextOptionsBuilder<TradingDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new TradingDbContext(optionsBuilder.Options);
        }
    }
}
