
using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;
using ILogger = Serilog.ILogger;

namespace Challenge02.Infraestructure.Messaging
{
    public class ServiceBusMessageSender : IServiceBusMessageSender
    {
        private readonly ServiceBusClient _client;
        private readonly string _queueName;
        private readonly ILogger _logger;

        public ServiceBusMessageSender(string connectionString, string queueName, ILogger logger)
        {
            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            _client = new ServiceBusClient(connectionString);
            _queueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendMessageAsync<T>(T messageObject)
        {
            try
            {
                var sender = _client.CreateSender(_queueName);
                var messageString = JsonConvert.SerializeObject(messageObject);
                var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageString));

                _logger.Information($"Sending message to queue {_queueName}");
                await sender.SendMessageAsync(message);
                _logger.Information("Message sent successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error sending message to queue {_queueName}");
                throw;
            }
        }
    }
}
