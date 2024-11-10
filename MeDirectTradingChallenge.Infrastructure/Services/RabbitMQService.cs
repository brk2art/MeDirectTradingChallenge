using MeDirectTradingChallenge.Domain.Models;
using MeDirectTradingChallenge.Domain.Services;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;

namespace MeDirectTradingChallenge.Infrastructure.Services
{
    public class RabbitMQService : IMessageQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(ConnectionFactory factory)
        {
            // Establish the connection and create a channel
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare the queue where the trades will be published
            _channel.QueueDeclare(
                queue: "trades",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
        }

        public Task PublishTrade(Trade trade)
        {
            try
            {
                // Serialize the trade object to JSON
                var message = JsonSerializer.Serialize(trade);
                var body = Encoding.UTF8.GetBytes(message);

                // Publish the message to the queue
                _channel.BasicPublish(
                    exchange: "",            // Default exchange
                    routingKey: "trades",    // Queue name as the routing key
                    basicProperties: null,
                    body: body);

                Console.WriteLine($"[x] Sent trade message: {message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error publishing trade: {ex.Message}");
                throw; // Rethrow the exception to handle it outside if needed
            }

            return Task.CompletedTask;
        }

        // Clean up resources
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}
