using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

using NatsApi.Health;
using NatsApi.Metric;

using Prometheus;

namespace NatsApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks().AddCheck<ApiHealthCheck>("Api Health Check");
            services.AddControllers();

            //Add a swagger generator
            services.AddSwaggerGen(options => options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "NATS Message API",
                Version = "v1",
                Description = "NATS Message ASP.NET Core Web API"
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHealthChecks("/healthy");
            app.UseMetricServer();
            app.UseMiddleware<RequestMiddleware>();

            //Include Swagger and Swagger UI in the stream
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "NATS Message API V1");
                options.RoutePrefix = "Swagger";
            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}