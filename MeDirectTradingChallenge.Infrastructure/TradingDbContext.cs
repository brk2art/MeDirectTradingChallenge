using MeDirectTradingChallenge.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MeDirectTradingChallenge.Infrastructure
{
    public class TradingDbContext : DbContext
    {
        public DbSet<Trade> Trades { get; set; }

        public TradingDbContext(DbContextOptions<TradingDbContext> options) : base(options) { }
    }
}
