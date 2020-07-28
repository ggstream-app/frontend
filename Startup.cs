using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using GGStream.Data;

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
            services.AddControllersWithViews();

            services.AddDbContext<Context>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("Context")));
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                /* Homepage */
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "/",
                    defaults: new
                    {
                        controller = "Home",
                        action = "Index"
                    });

                /* Admin/Default Entrypoints */
                endpoints.MapControllerRoute(
                    name: "admin",
                    pattern: "/Admin/{controller=Admin}/{action=Index}/{id?}");

                /* Collection Entrypoint */
                endpoints.MapControllerRoute(
                    name: "collection",
                    pattern: "{url}",
                    defaults: new
                    {
                        controller = "Collections",
                        action = "ViewStream"
                    });

                /* Stream Entrypoint */
                endpoints.MapControllerRoute(
                    name: "stream",
                    pattern: "{url}/{id}",
                    defaults: new
                    {
                        controller = "Streams",
                        action = "ViewStream"
                    });
            });
        }
    }
}
