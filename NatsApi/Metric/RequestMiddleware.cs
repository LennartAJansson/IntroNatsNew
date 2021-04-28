using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Prometheus;

using System;
using System.Threading.Tasks;

namespace NatsApi.Metric
{
    //https://www.c-sharpcorner.com/article/reporting-metrics-to-prometheus-in-asp-net-core/
    //https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1
    public class RequestMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger logger;
        private readonly Counter totalRequests;
        private readonly Counter okRequests;
        private readonly Counter exceptionRequests;

        public RequestMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            logger = loggerFactory.CreateLogger<RequestMiddleware>();
            //This counter count all requests
            totalRequests = Metrics.CreateCounter("prometheus_request_total", "HTTP Requests Total", new CounterConfiguration
            {
                LabelNames = new[] { "path", "method", "status" }
            });

            //This counter count all ok requests
            okRequests = Metrics.CreateCounter("prom_ok", "This fields indicates the transactions that were processed correctly.", new CounterConfiguration
            {
                LabelNames = new[] { "path", "method", "status" }
            });

            //This counter count all request ending in exception
            exceptionRequests = Metrics.CreateCounter("prom_exception", "This fields indicates the exception count.", new CounterConfiguration
            {
                LabelNames = new[] { "path", "method", "status" }
            });
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string path = httpContext.Request.Path.Value;
            string method = httpContext.Request.Method;

            //Set default http statuscode
            int statusCode = 200;

            if (path != "/metrics")
            {
                try
                {
                    logger.LogInformation("------------------------------------------------------------------");
                    logger.LogInformation($"Calling the controller {method} {path}");
                    //Call down the chain (will end up in the controller endpoints)
                    await next.Invoke(httpContext);
                    statusCode = httpContext.Response.StatusCode;
                    okRequests.Labels(path, method, statusCode.ToString()).Inc();
                    logger.LogInformation($"Returned from the controller {method} {path}");
                    logger.LogInformation("------------------------------------------------------------------");
                }
                catch (Exception)
                {
                    statusCode = 500;
                    exceptionRequests.Labels(path, method, statusCode.ToString()).Inc();
                    throw;
                }
                finally
                {
                    totalRequests.Labels(path, method, statusCode.ToString()).Inc();
                }
            }
        }
    }
}