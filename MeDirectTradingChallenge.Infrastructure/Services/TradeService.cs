using MeDirectTradingChallenge.Domain.Models;
using MeDirectTradingChallenge.Domain.Repositories;
using MeDirectTradingChallenge.Domain.Services;

namespace MeDirectTradingChallenge.Infrastructure.Services
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IMessageQueueService _messageQueueService;
        private readonly ICacheService _cacheService;
        private const string TradeCacheKey = "trades";

        public TradeService(ITradeRepository tradeRepository,
                            IMessageQueueService messageQueueService,
                            ICacheService cacheService)
        {
            _tradeRepository = tradeRepository;
            _messageQueueService = messageQueueService;
            _cacheService = cacheService;
        }

        public async Task ExecuteTrade(Trade trade)
        {
            await _tradeRepository.AddTradeAsync(trade);
            await _messageQueueService.PublishTrade(trade);

            // Clear the cached list of trades after a new trade is executed
            await _cacheService.RemoveCacheAsync(TradeCacheKey);
        }

        public async Task<IEnumerable<Trade>> GetTrades()
        {
            // Try to retrieve trades from cache
            var cachedTrades = await _cacheService.GetCacheAsync<IEnumerable<Trade>>(TradeCacheKey);

            if (cachedTrades != null)
            {
                return cachedTrades;
            }

            // If not in cache, retrieve from repository and cache the result
            var trades = await _tradeRepository.GetTradesAsync();
            await _cacheService.SetCacheAsync(TradeCacheKey, trades, TimeSpan.FromMinutes(10));

            return trades;
        }
    }
}
