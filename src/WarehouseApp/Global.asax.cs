using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WarehouseApp.Data;

namespace WarehouseApp
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var key = ConfigurationManager.AppSettings["APPLICATION_INSIGHTS_IKEY"];
            if (!string.IsNullOrWhiteSpace(key))
            {
                TelemetryConfiguration.Active.InstrumentationKey = key;
            }

            try
            {
                Database.SetInitializer(new WarehouseAppDbContextSeedInitializer());
                using (var context = new WarehouseAppDbContext())
                {
                    context.Database.Initialize(true);
                }
            }
            catch (Exception ex)
            {
                var client = new TelemetryClient();
                client.TrackException(new ExceptionTelemetry
                {
                    Exception = ex,
                    Message = "Startup failure with Warehouse App database",
                    SeverityLevel = SeverityLevel.Critical
                });
            }
        }
    }
}
