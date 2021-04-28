using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

using System.Threading;
using System.Threading.Tasks;

namespace NatsApi.Health
{
    internal class ApiHealthCheck : IHealthCheck
    {
        private readonly ILogger<ApiHealthCheck> logger;

        public ApiHealthCheck(ILogger<ApiHealthCheck> logger)
        {
            this.logger = logger;
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Checking health");

            bool healthCheckResultHealthy = true;//Change to some class that tests health on all systems

            if (healthCheckResultHealthy)
            {
                return Task.FromResult(
                    HealthCheckResult.Healthy("The check indicates a healthy result."));
            }

            //Also supports HealthCheckResult.Degraded

            return Task.FromResult(
                HealthCheckResult.Unhealthy("The check indicates an unhealthy result."));
        }
    }
}