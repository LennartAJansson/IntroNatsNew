﻿using CloudNative.CloudEvents;

using Microsoft.Extensions.Logging;

using NATS.Client;

using NatsApi.Nats;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NatsApi.Services
{
    public class Class
    {
    }

    public interface INatsService
    {
        Task SendAsync(CloudEvent cloudEvent);
    }

    public class NatsService : INatsService
    {
        private readonly ILogger<NatsService> logger;
        private readonly NatsConnection natsConnection;

        public NatsService(ILogger<NatsService> logger, NatsConnection natsConnection)
        {
            this.logger = logger;
            this.natsConnection = natsConnection;
        }

        public async Task SendAsync(CloudEvent cloudEvent)
        {
            IConnection connection = natsConnection.GetConnection();
            connection.ResetStats();
            string json = JsonSerializer.Serialize(cloudEvent);
            Msg msg = await connection.RequestAsync($"natssamplestream.samplesubject.timestamp.received.{cloudEvent.Id}", Encoding.UTF8.GetBytes(json), 10000);
            string decodedMessage = Encoding.UTF8.GetString(msg.Data);
            logger.LogInformation($"Response from NATS is: {decodedMessage}");
        }
    }
}