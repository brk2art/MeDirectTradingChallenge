using MeDirectTradingChallenge.Domain.Models;

namespace MeDirectTradingChallenge.Domain.Repositories
{
    public interface ITradeRepository
    {
        Task AddTradeAsync(Trade trade);
        Task<IEnumerable<Trade>> GetTradesAsync();
    }
}
