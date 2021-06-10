using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using NATS.Client;

using NatsConsumer.Nats;

using Prometheus;

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NatsConsumer
{
    public class Worker : IHostedService
    {
        private readonly ILogger<Worker> logger;
        private readonly NatsConnection natsConnection;
        private IConnection connection;
        private IAsyncSubscription subscription;
        private readonly Counter eventCount;
        private readonly Gauge eventExecuteTime;

        public Worker(ILogger<Worker> logger, NatsConnection natsConnection)
        {
            this.logger = logger;
            this.natsConnection = natsConnection;
            eventExecuteTime = Metrics.CreateGauge("nats_event_execution_time", "Counts total execution time for handling NATS message");
            eventCount = Metrics.CreateCounter("nats_event_total", "NATS Events Total", new CounterConfiguration
            {
                LabelNames = new[] { "type", "status" }
            });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            connection = natsConnection.GetConnection();
            logger.LogInformation("---------------------------------------------------------------");
            logger.LogInformation("-->Connecting to natssamplestream...");
            subscription = connection.SubscribeAsync("natssamplestream.samplesubject.timestamp.received.>");
            subscription.MessageHandler += SubscriptionArrived;
            logger.LogInformation("-->Connected to natssamplestream!");
            logger.LogInformation("---------------------------------------------------------------");
            subscription.Start();
            return Task.CompletedTask;
        }

        private void SubscriptionArrived(object sender, MsgHandlerEventArgs e)
        {
            DateTime startDateTime = DateTime.Now;
            logger.LogInformation("---------------------------------------------------------------");
            logger.LogInformation("-->New message has arrived in natssamplestream!");
            logger.LogInformation(e.Message.GetDataString());
            logger.LogInformation("-->ACK successfully sent to natssamplestream!");
            logger.LogInformation("---------------------------------------------------------------");
            e.Message.Respond(Encoding.UTF8.GetBytes("+OK"));
            eventCount.Labels("Event arrived", "+OK").Inc();
            DateTime endDateTime = DateTime.Now;
            eventExecuteTime.Set((endDateTime - startDateTime).TotalMilliseconds);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            subscription.Unsubscribe();
            connection.Close();
            return Task.CompletedTask;
        }
    }
}