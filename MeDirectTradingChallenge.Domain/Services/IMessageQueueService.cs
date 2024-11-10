using MeDirectTradingChallenge.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeDirectTradingChallenge.Domain.Services
{
    public interface IMessageQueueService
    {
        Task PublishTrade(Trade trade);
    }
}
