using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriceProviderService.SignalR;

namespace PriceProviderService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<ILogger>(logger => logger.GetRequiredService<ILogger<FileReaderService>>());
            services.AddHostedService<Worker>();
            services.AddSingleton<FileReaderService>();
            services.AddSingleton<IConfiguration>(provider =>
                                                    new ConfigurationBuilder()
                                                    .AddEnvironmentVariables()
                                                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).Build());

            services.AddCors(options =>
            {
                options.AddPolicy("SpaClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("SpaClientPermission");
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<PricesHub>("/hubs/prices");
            });
        }
    }
}
