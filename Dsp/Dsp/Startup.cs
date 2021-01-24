using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dsp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Dsp
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
            //services.AddDbContext<DspContext>(opt=> opt.UseInMemoryDatabase("DespesasList"));
            //var connection = @"Data Source=(localdb)\MSSQLLocalDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            //var connection = "'Data Source = (localdb)\\MSSQLLocalDB;Initial;Integrated Security = true' Provider name = 'System.Data.SqlClient'";
            /*services.AddDbContext < DspContext>(options =>
                  options.UseInMemoryDatabase("DspList")
            ) ;*/

            services.AddDbContext<DspContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("con4"))
            );

           /* services.Configure<IISOptions>(o =>
            {
                o.ForwardClientCertificate = false;
            });*/

            services.AddControllers();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
