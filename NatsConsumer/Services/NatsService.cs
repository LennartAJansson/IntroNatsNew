using CloudNative.CloudEvents;

using Microsoft.Extensions.Logging;

using NATS.Client;

using NatsConsumer.Nats;

using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NatsConsumer.Services
{
    public class NatsService : INatsService
    {
        private readonly ILogger<NatsService> logger;
        private readonly NatsConnection natsConnection;

        public NatsService(ILogger<NatsService> logger, NatsConnection natsConnection)
        {
            this.logger = logger;
            this.natsConnection = natsConnection;
        }

        public async Task SendAsync(string subject, CloudEvent cloudEvent)
        {
            IConnection connection = natsConnection.GetConnection();
            connection.ResetStats();
            string json = JsonSerializer.Serialize(cloudEvent);
            Msg msg = await connection.RequestAsync($"{subject}.{cloudEvent.Id}", Encoding.UTF8.GetBytes(json), 10000);
            string decodedMessage = Encoding.UTF8.GetString(msg.Data);
            logger.LogInformation($"Response from NATS is: {decodedMessage}");
        }
    }
}