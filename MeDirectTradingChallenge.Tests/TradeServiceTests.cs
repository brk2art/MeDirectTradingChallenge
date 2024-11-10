using MeDirectTradingChallenge.Domain.Models;
using MeDirectTradingChallenge.Domain.Repositories;
using MeDirectTradingChallenge.Domain.Services;
using MeDirectTradingChallenge.Infrastructure.Services;
using Moq;

namespace MeDirectTradingChallenge.Tests
{
    public class TradeServiceTests
    {
        private readonly Mock<ITradeRepository> _mockTradeRepository;
        private readonly Mock<IMessageQueueService> _mockMessageQueueService;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly TradeService _tradeService;

        private const string TradeCacheKey = "trades";

        public TradeServiceTests()
        {
            _mockTradeRepository = new Mock<ITradeRepository>();
            _mockMessageQueueService = new Mock<IMessageQueueService>();
            _mockCacheService = new Mock<ICacheService>();

            _tradeService = new TradeService
            (
                _mockTradeRepository.Object,
                _mockMessageQueueService.Object,
                _mockCacheService.Object
            );
        }

        [Fact]
        public async Task ExecuteTrade_ShouldAddTradeAndPublishToQueueAndClearCache()
        {
            var trade = new Trade { Id = 1, Amount = 100, RecordTime = DateTime.UtcNow };

            await _tradeService.ExecuteTrade(trade);

            _mockTradeRepository.Verify(repo => repo.AddTradeAsync(trade), Times.Once);
            _mockMessageQueueService.Verify(queue => queue.PublishTrade(trade), Times.Once);
            _mockCacheService.Verify(cache => cache.RemoveCacheAsync(TradeCacheKey), Times.Once);
        }

        [Fact]
        public async Task GetTrades_ShouldReturnCachedTrades_WhenCacheIsNotEmpty()
        {
            var cachedTrades = new List<Trade>
            {
                new Trade { Id = 1, Amount = 100, RecordTime = DateTime.UtcNow },
                new Trade { Id = 2, Amount = 50, RecordTime = DateTime.UtcNow }
            };

            _mockCacheService.Setup(cache => cache.GetCacheAsync<IEnumerable<Trade>>(TradeCacheKey)).ReturnsAsync(cachedTrades);

            var result = await _tradeService.GetTrades();

            Assert.Equal(cachedTrades, result);
            _mockTradeRepository.Verify(repo => repo.GetTradesAsync(), Times.Never);
            _mockCacheService.Verify(cache => cache.GetCacheAsync<IEnumerable<Trade>>(TradeCacheKey), Times.Once);
        }

        [Fact]
        public async Task GetTrades_ShouldRetrieveFromRepositoryAndCacheResult_WhenCacheIsEmpty()
        {
            // Arrange
            var repositoryTrades = new List<Trade>
            {
                new Trade { Id = 1, Amount = 100, RecordTime = DateTime.UtcNow },
                new Trade { Id = 2, Amount = 50, RecordTime = DateTime.UtcNow }
            };

            _mockCacheService.Setup(cache => cache.GetCacheAsync<IEnumerable<Trade>>(TradeCacheKey)).ReturnsAsync((IEnumerable<Trade>)null); // Cache is empty

            _mockTradeRepository.Setup(repo => repo.GetTradesAsync())
                                .ReturnsAsync(repositoryTrades);

            var result = await _tradeService.GetTrades();

            Assert.Equal(repositoryTrades, result);
            _mockTradeRepository.Verify(repo => repo.GetTradesAsync(), Times.Once);
            _mockCacheService.Verify(cache => cache.SetCacheAsync(TradeCacheKey, repositoryTrades, It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}
