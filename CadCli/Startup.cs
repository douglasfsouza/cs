using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CadCli.Core.Data.ADO;
using CadCli.Core.Data.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CadCli
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<CadCliDbContext>();
            services.AddTransient<CadCli.Core.Contracts.IRepository, RepositoryEF>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration config)
        {
            bool man = Convert.ToBoolean( config["man"]);
            if (man)
            {
                app.Run(async (ctx) =>
                {
                    await ctx.Response.WriteAsync("Em manutencao");
                });

            }


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                
            });
        }

        
    }
    
}
