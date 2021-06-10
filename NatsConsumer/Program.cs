using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NatsConsumer.Configurations;
using NatsConsumer.Services;

namespace NatsConsumer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build()/*.TestConnection()*/.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => services
                    .AddApplicationConfigurations(hostContext)
                    .AddApplicationServices()
                    .AddHostedService<Worker>())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}