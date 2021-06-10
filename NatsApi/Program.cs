using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using NatsApi.Configurations;
using NatsApi.Services;

namespace NatsApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build()/*.TestConnection()*/.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) => services
                    .AddApplicationConfigurations(hostContext)
                    .AddApplicationServices())
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
        }
    }
}