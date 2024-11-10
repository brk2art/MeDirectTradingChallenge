using MeDirectTradingChallenge.Domain.Models;
using MeDirectTradingChallenge.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MeDirectTradingChallenge.Infrastructure.Repositories
{
    public class TradeRepository : ITradeRepository
    {
        private readonly TradingDbContext _context;

        public TradeRepository(TradingDbContext context)
        {
            _context = context;
        }

        public async Task AddTradeAsync(Trade trade)
        {
            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Trade>> GetTradesAsync()
        {
            return await _context.Trades.ToListAsync();
        }
    }
}
