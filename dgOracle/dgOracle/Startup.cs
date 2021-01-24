using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess;
using Oracle.ManagedDataAccess.Client;

namespace dgOracle
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Acessando AA2CPARA!\n");

                    string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.0.117)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=RMS)));User Id=varsis;Password=pwdv4r51s123";
                    OracleConnection con = new OracleConnection(connectionString);
                    con.Open();
                    OracleCommand cmd = con.CreateCommand();
                    cmd.CommandText = "Select * from carone.AA2CPARA";

                    OracleDataReader reader = cmd.ExecuteReader();
                    await context.Response.WriteAsync("Codigo\tAcesso\tConteudo\n");

                    while (reader.Read())
                    {
                        await context.Response.WriteAsync($"{reader.GetString(0)}\t{reader.GetString(1)}\t{reader.GetString(2)}\n");

                    }
                });
            });
        }
    }
}
