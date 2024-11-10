using MeDirectTradingChallenge.Domain.Models;
using MeDirectTradingChallenge.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace MeDirectTradingChallenge.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradesController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpPost]
        public async Task<IActionResult> ExecuteTrade([FromBody] Trade trade)
        {
            await _tradeService.ExecuteTrade(trade);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetTrades()
        {
            var trades = await _tradeService.GetTrades();
            return Ok(trades);
        }
    }
}
