using System.Net;
using BadBroker.Configs;
using BadBroker.Infrastructure;
using BadBroker.Infrastructure.Exceptions;
using BadBroker.Services.ExchangeRateIntegration;
using BadBroker.Services.ExchangeRateIntegration.ExchangeRatesApiIoIntegration;
using BadBroker.Services.RevenueCalculation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace BadBroker
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BadBroker", Version = "v1" });
            });

            services.AddSingleton<IExchangeRateIntegrationService, ExchangeRatesApiIoIntegrationService>();
            services.AddSingleton<RevenueCalculatorService, RevenueCalculatorService>();

            services.AddLogging(builder => builder.AddSimpleConsole());

            services.AddOptions<ExchangeRatesApiIoOptions>()
                .BindConfiguration(ExchangeRatesApiIoOptions.ExchangeRatesApiIo);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BadBroker v1"));
            }
            
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    var loggerFactory = context.RequestServices.GetService<ILoggerFactory>();
                    var logger = loggerFactory.CreateLogger("ErrorMiddleware");
                    
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";
                    
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if(contextFeature != null)
                    { 
                        logger.LogError($"{contextFeature.Error}");

                        var responseErrorMessage = "Internal Server Error";

                        if (contextFeature.Error is ValidationException)
                            responseErrorMessage += ": " + contextFeature.Error.Message;
                        
                        await context.Response.WriteAsync(new ErrorDetails()
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = responseErrorMessage
                        }.ToString());
                    }
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}