using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace NatsApi.Configurations
{
    public static class ApplicationConfigurationExtension
    {
        public static IServiceCollection AddApplicationConfigurations(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            return services;
        }
    }
}
