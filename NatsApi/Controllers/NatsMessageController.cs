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
            var scheme = HttpContext.Request.Scheme;
            var host = HttpContext.Request.Host.Value;
            var path = HttpContext.Request.Path.Value;
            var url = $"{scheme}://{host}{path}";
            CloudEvent evt = new(CloudEventsSpecVersion.Default, type: nameof(NatsPostMessage), source: new Uri(url))
            {
                DataContentType = new ContentType("application/json; charset=UTF-8"),
                //evt.DataSchema
                //evt.Extensions
                Subject = "natssamplestream.samplesubject.timestamp.received",
                Data = message
            };
            await service.SendAsync(evt);
        }
    }
}