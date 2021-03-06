﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using GGStream.Data;
using GGStream.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

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
            services.AddMicrosoftWebAppAuthentication(Configuration);
            services.AddRazorPages().AddMicrosoftIdentityUI();

            // Database
            services.AddDbContext<Context>(options =>
                options.UseSqlite(Configuration.GetConnectionString("Context")));

            // App Insights
            services.AddApplicationInsightsTelemetry();

            // Custom Services
            services.Add(new ServiceDescriptor(typeof(IApplicationDateTime), new ApplicationDateTime(Configuration)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger,
            IApplicationDateTime adt)
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

            LogAppStartup(logger, adt);

            app.UseStatusCodePagesWithReExecute("/error", "?code={0}");

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        public void LogAppStartup(ILogger<Startup> logger, IApplicationDateTime adt)
        {
            logger.LogInformation("=== GGStream.app Frontend - Startup");

            logger.LogInformation("\n--- Configured Endpoints");
            logger.LogInformation("Ingest: {Ingest}", Configuration.GetValue<string>("IngestEndpoint"));
            Configuration.GetSection("SvcInstances").Get<List<SvcInstance>>().ForEach(i =>
            {
                logger.LogInformation(
                    "Service: {Service} / {Endpoint} / Secure: {Secure}, WebRTC: {WebRTC}, DASH: {DASH}, HLS {HLS}",
                    i.Name, i.Endpoint, i.Secure, i.Protocols.WebRTC, i.Protocols.DASH, i.Protocols.HLS);
            });

            logger.LogInformation("\n--- Application DateTime");
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(Configuration.GetValue<string>("TimeZone"));

            logger.LogInformation("System time: {Time}", DateTime.Now.ToString(CultureInfo.InvariantCulture));
            logger.LogInformation("UTC time: {TimeUTC}", DateTime.UtcNow.ToString(CultureInfo.InvariantCulture));
            logger.LogInformation("TimeZone: {TZ}", Configuration.GetValue<string>("TimeZone"));
            logger.LogInformation("Base Offset: {TZI} / DST: {DST}", tzi.BaseUtcOffset,
                tzi.IsDaylightSavingTime(DateTime.Now));
            logger.LogInformation("ApplicationDateTime: {ADTime}", adt.Now().ToString(CultureInfo.InvariantCulture));

            logger.LogInformation("\n--- AAD Authentication");
            logger.LogInformation("Domain: {Domain}", Configuration.GetValue<string>("AzureAd:Domain"));
            logger.LogInformation("TenantId: {TenantId}", Configuration.GetValue<string>("AzureAd:TenantId"));
            logger.LogInformation("ClientId: {ClientId}", Configuration.GetValue<string>("AzureAd:ClientId"));

            logger.LogInformation("\n--- Application Insights");
            logger.LogInformation("Instrumentation Key: {IKey}",
                Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey"));
        }
    }
}