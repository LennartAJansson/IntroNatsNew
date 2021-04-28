using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NatsConsumer.Nats;

using System;

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
                .ConfigureServices((hostContext, services) =>
                {
                    Console.WriteLine($"Environment says: {Environment.GetEnvironmentVariable("NATS_URL")}");
                    NatsConnection natsConnection = new NatsConnection(Environment.GetEnvironmentVariable("NATS_URL"));
                    services.AddSingleton<NatsConnection>(natsConnection);
                    services.AddHostedService<Worker>();
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());
    }
}