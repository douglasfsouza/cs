using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using Newtonsoft.Json;

namespace dgOData
{
    public class Startup
    {
        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://localhost:44354/weatherforecast",
                                                          "http://www.contoso.com",
                                                          "http://127.0.0.1:5500");
                                  });
            });

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            //services.AddMvc(options => options.EnableEndpointRouting = false);
            //adicionando o data
            services.AddMvc(opc => opc.EnableEndpointRouting = false ).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            var connection = @"Data Source=(LocalDb)\MSSQLLocalDB;Initial Catalog=Escola;Integrated Security=True";
            services.AddDbContext<Models.AppDbContext>(options => options.UseSqlServer(connection));
            //services.AddControllers();
            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.Expand().Select().Count().OrderBy().Filter();
                
            });

            app.UseRouting();

            //app.UseCors(MyAllowSpecificOrigins);

            app.UseCors(x =>
            {
                x.AllowAnyMethod();
                x.AllowAnyHeader();
                x.AllowAnyOrigin();
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapODataRoute("odata", "odata", GetEdmModel());
            });
        }

        private IEdmModel GetEdmModel()
        {
            throw new NotImplementedException();
        }
    }
}
