using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NATS.Client;

namespace NatsApi.Nats
{
    public static class NatsExtensions
    {
        public static IHost TestConnection(this IHost host)
        {
            IConnection connection = host.Services.GetRequiredService<NatsConnection>().GetConnection();
            connection.ResetStats();
            return host;
        }
    }
}