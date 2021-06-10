using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NATS.Client;

namespace NatsApi.Nats
{
    public static class NatsExtensions
    {
        public static IHost TestConnection(this IHost host)
        {
            using (IServiceScope scope = host.Services.CreateScope())
            {
                IConnection connection = scope.ServiceProvider.GetRequiredService<NatsConnection>().GetConnection();
                connection.ResetStats();
            }
            return host;
        }
    }
}