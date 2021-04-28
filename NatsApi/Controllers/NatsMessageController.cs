using CloudNative.CloudEvents;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Model;

using NatsApi.Services;

using System;
using System.Net.Mime;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NatsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NatsMessageController : ControllerBase
    {
        private readonly ILogger<NatsMessageController> logger;
        private readonly INatsService service;

        public NatsMessageController(ILogger<NatsMessageController> logger, INatsService service)
        {
            this.logger = logger;
            this.service = service;
        }

        // POST api/<NatsMessage>
        [HttpPost]
        public async Task PostAsync([FromBody] NatsPostMessage message)
        {
            var scheme = this.HttpContext.Request.Scheme;
            var host = this.HttpContext.Request.Host.Value;
            var path = this.HttpContext.Request.Path.Value;
            var url = $"{scheme}://{host}{path}";
            CloudEvent evt = new CloudEvent(CloudEventsSpecVersion.Default, type: nameof(NatsPostMessage), source: new Uri(url))
            {
                DataContentType = new ContentType("application/json; charset=UTF-8"),

                //evt.DataSchema
                //evt.Extensions
                Subject = "natssamplestream.samplesubject.timestamp.received",
                Data = message
            };
            await service.SendAsync(evt);
        }

        //[HttpGet]
        //public ContentResult Index()
        //{
        //    IConnection connection = natsConnection.GetConnection();
        //    connection.Publish($"natssamplestream.samplesubject.timestamp.received.{(DateTime.Now).ToString("yyyyMMddHHmmssffff")}", Encoding.UTF8.GetBytes("[MESSAGE]: Hello!, Sent: " + DateTimeOffset.Now));
        //    logger.LogInformation("-->Sending data to 'natssamplestream' at: " + DateTimeOffset.Now);

        //    return new ContentResult
        //    {
        //        ContentType = "text/html",
        //        Content = $"<head><title>NatsMessageController</title></head><h1>NatsMessageController</h1><p>[MESSAGE]: Hello!, Sent: {DateTimeOffset.Now} to 'natssamplestream'</p>"
        //    };
        //}
    }
}