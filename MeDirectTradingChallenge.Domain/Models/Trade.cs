namespace MeDirectTradingChallenge.Domain.Models
{
    public class Trade
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime RecordTime { get; set; }
    }
}
