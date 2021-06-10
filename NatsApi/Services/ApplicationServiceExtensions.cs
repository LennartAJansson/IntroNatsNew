
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NatsApi.Nats;

namespace NatsApi.Services
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(sp => new NatsConnection(sp.GetRequiredService<IConfiguration>().GetValue<string>("NATS_URL")));
            services.AddTransient<INatsService, NatsService>();
            return services;
        }
    }
}