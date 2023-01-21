using oDataOnly.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Batch;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;
using System.Web.Http.OData.Routing;
using System.Web.Http.Cors;

namespace oDataOnly
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Serviços e configuração da API da Web

            // Rotas da API da Web

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();

            //tst
            //config.Count().Filter().OrderBy().Expand().Select().MaxTop(null);

             var countries = builder.EntitySet<Country>("Countries");
            // countries.EntityType.Count().Filter().OrderBy().Expand().Select();
            config.AddODataQueryFilter();
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            //builder.EntityType<Country>().Filter("Code");
            //fim teste



            builder.EntitySet<Country>("Countries");

            config.Routes.MapODataRoute("odata", "odata", builder.GetEdmModel());

            config.MapHttpAttributeRoutes();

           

            var cors_allow_origins = ConfigurationManager.AppSettings["cors_allow_origins"];

            var cors = new System.Web.Http.Cors.EnableCorsAttribute(cors_allow_origins, "*", "*");

            cors.SupportsCredentials = true;

            config.EnableCors(cors);

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            //var edmModel = WebApiConfig.InitializeODataModel();
            //var odataBatchHandler = new DefaultODataBatchHandler(GlobalConfiguration.DefaultServer);

            //config.Routes.MapODataServiceRoute(
            //    routeName: "ODataRoute",
            //    routePrefix: "s3/v1",
            //    model: edmModel,
            //    batchHandler: odataBatchHandler,
            //    routingConventions: conventions,
            //    pathHandler: new DefaultODataPathHandler()
            //);

            //config.AddODataQueryFilter();
        }
    }
}
