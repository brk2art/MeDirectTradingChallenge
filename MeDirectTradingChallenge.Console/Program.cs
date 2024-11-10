using log4net;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
public class TradeConsumer
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(TradeConsumer));

    public static void Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        Logger.Info("Starting RabbitMQ Consumer...");

        try
        {
            var rabbitMqConfig = configuration.GetSection("RabbitMQ");
            var factory = new ConnectionFactory()
            {
                HostName = rabbitMqConfig["HostName"],
                UserName = rabbitMqConfig["UserName"],
                Password = rabbitMqConfig["Password"]
            };

            using var connection = factory.CreateConnection();
            Logger.Info("Connected to RabbitMQ");

            using var channel = connection.CreateModel();
            Logger.Info("Channel created");

            channel.QueueDeclare(queue: "trades", durable: false, exclusive: false, autoDelete: false, arguments: null);
            Logger.Info("Queue 'trades' declared");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Logger.Info($"Received message: {message}");
            };

            channel.BasicConsume(queue: "trades", autoAck: true, consumer: consumer);
            Logger.Info("Started consuming messages from 'trades' queue");

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
            Logger.Info("Application exiting");
        }
        catch (Exception ex)
        {
            Logger.Error("An error occurred in the RabbitMQ consumer", ex);
        }
    }
}