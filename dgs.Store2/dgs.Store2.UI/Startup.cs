using dgs.store.Data.EF;
using dgs.store.Data.EF.Repositories;
using dgs.Store.Domain.Contracts.Repositories;
using dgs.Store.IocDI;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace dgs.Store2.UI
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.SetupServices();
            services.AddAuthentication(
                options => {
                    options.DefaultChallengeScheme =
                    options.DefaultSignInScheme =
                    options.DefaultAuthenticateScheme =
                    CookieAuthenticationDefaults.AuthenticationScheme;
                   }).AddCookie(options => {
                       options.LoginPath = "/Auth/login";
                       });
         }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            var cultureInfo = new CultureInfo("pt-BR");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseEndpoints(endpoints =>
            {
                //{controller=home}/{action=index}/id
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
