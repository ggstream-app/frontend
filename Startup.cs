using GGStream.Data;
using GGStream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using System;
using System.Net;

namespace GGStream
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
            // Reverse Proxy Forwarding
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                
                /* Add Docker default bridge */
                options.KnownNetworks.Add(new IPNetwork(IPAddress.Parse("::ffff:172.17.0.0"), 24));
            });

            // AAD Auth
            services.AddMicrosoftWebAppAuthentication(Configuration, "AzureAd");
            services.AddRazorPages().AddMicrosoftIdentityUI();

            // Database
            services.AddDbContext<Context>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("Context")));

            // App Insights
            services.AddApplicationInsightsTelemetry(Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"]);

            // Custom Services
            services.Add(new ServiceDescriptor(typeof(IApplicationDateTime), new ApplicationDateTime(Configuration)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger, IApplicationDateTime adt)
        {
            // Automatically apply DB migrations on startup.
            using (var serviceScope = app.ApplicationServices
            .GetRequiredService<IServiceScopeFactory>()
            .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<Context>())
                {
                    context.Database.Migrate();
                }
            }

            app.UseForwardedHeaders();

            // Log Forwarding requests
            app.Use(async (context, next) =>
            {
                // Request method, scheme, and path
                logger.LogDebug("Request Method: {Method}", context.Request.Method);
                logger.LogDebug("Request Scheme: {Scheme}", context.Request.Scheme);
                logger.LogDebug("Request Path: {Path}", context.Request.Path);

                // Headers
                foreach (var header in context.Request.Headers)
                {
                    logger.LogDebug("Header: {Key}: {Value}", header.Key, header.Value);
                }

                // Connection: RemoteIp
                logger.LogDebug("Request RemoteIp: {RemoteIpAddress}",
                    context.Connection.RemoteIpAddress);

                await next();
            });

            if (env.IsDevelopment())
            {
                logger.LogInformation("Development Mode");
                app.UseDeveloperExceptionPage();

                // Production will be hosted behind a reverse proxy, we only need to enable redirection locally
                app.UseHttpsRedirection();
            }
            else
            {
                logger.LogInformation("Production Mode");
                app.UseExceptionHandler("/exception");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            logger.LogDebug("Current time: {Time}", DateTime.Now.ToString());
            logger.LogDebug("Current UTC time: {TimeUTC}", DateTime.UtcNow.ToString());
            logger.LogDebug("Current tzi: {TZI}", TimeZoneInfo.FindSystemTimeZoneById("America/Los_Angeles"));
            logger.LogDebug("Current ApplicationDateTime: {ADTime}", adt.Now().ToString());

            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
