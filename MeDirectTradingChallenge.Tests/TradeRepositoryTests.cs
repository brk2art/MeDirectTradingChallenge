using MeDirectTradingChallenge.Domain.Models;
using MeDirectTradingChallenge.Infrastructure;
using MeDirectTradingChallenge.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeDirectTradingChallenge.Tests
{
    public class TradeRepositoryTests
    {
        private readonly DbContextOptions<TradingDbContext> _dbOptions;
        private readonly TradingDbContext _context;
        private readonly TradeRepository _repository;

        public TradeRepositoryTests()
        {
            // Configure the in-memory database
            _dbOptions = new DbContextOptionsBuilder<TradingDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Create a new context and repository for each test
            _context = new TradingDbContext(_dbOptions);
            _repository = new TradeRepository(_context);
        }

        [Fact]
        public async Task AddTradeAsync_ShouldAddTradeToDatabase()
        {
            // Arrange
            var trade = new Trade { Id = 1, Amount = 10, RecordTime = DateTime.UtcNow };

            // Act
            await _repository.AddTradeAsync(trade);
            var result = await _context.Trades.FindAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(trade.Amount, result.Amount);
        }

        [Fact]
        public async Task GetTradesAsync_ShouldReturnAllTrades()
        {
            // Arrange
            var trades = new List<Trade>
            {
                new Trade { Id = 1, Amount = 10, RecordTime = DateTime.UtcNow },
                new Trade { Id = 2, Amount = 20, RecordTime = DateTime.UtcNow }
            };

            await _context.Trades.AddRangeAsync(trades);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetTradesAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, t => t.Amount == 10);
            Assert.Contains(result, t => t.Amount == 20);
        }
    }
}
