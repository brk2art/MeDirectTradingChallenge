using MeDirectTradingChallenge.Domain.Models;
using MeDirectTradingChallenge.Infrastructure.Services;
using Moq;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MeDirectTradingChallenge.Tests
{
    public class RabbitMQServiceTests
    {
        private readonly Mock<IConnection> _mockConnection;
        private readonly Mock<IModel> _mockChannel;
        private readonly Mock<ConnectionFactory> _mockFactory;
        private readonly RabbitMQService _rabbitMqService;

        public RabbitMQServiceTests()
        {
            _mockConnection = new Mock<IConnection>();
            _mockChannel = new Mock<IModel>();

            _mockFactory = new Mock<ConnectionFactory>();
            _mockFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);

            _mockConnection.Setup(conn => conn.CreateModel()).Returns(_mockChannel.Object);

            _rabbitMqService = new RabbitMQService(_mockFactory.Object);
        }

        [Fact]
        public async Task PublishTrade_ShouldSendMessageToQueue()
        {
            var trade = new Trade { Id = 1, Amount = 100, RecordTime = DateTime.UtcNow };
            var expectedMessage = JsonSerializer.Serialize(trade);
            var expectedBody = Encoding.UTF8.GetBytes(expectedMessage);

            await _rabbitMqService.PublishTrade(trade);

            _mockChannel.Verify(ch => ch.BasicPublish(
                "",
                "trades",
                It.IsAny<IBasicProperties>(),
                It.Is<byte[]>(b => b.SequenceEqual(expectedBody))
            ), Times.Once);

            _mockChannel.Verify(ch => ch.QueueDeclare(
                "trades",   
                false,      
                false,      
                false,      
                null        
            ), Times.Once);
        }

        [Fact]
        public void Dispose_ShouldCloseChannelAndConnection()
        {
            _rabbitMqService.Dispose();

            _mockChannel.Verify(ch => ch.Close(), Times.Once);
            _mockConnection.Verify(conn => conn.Close(), Times.Once);
        }
    }
}
