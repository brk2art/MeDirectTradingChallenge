using MeDirectTradingChallenge.Domain.Models;

namespace MeDirectTradingChallenge.Domain.Services
{
    public interface ITradeService
    {
        Task ExecuteTrade(Trade trade);
        Task<IEnumerable<Trade>> GetTrades();
    }
}
