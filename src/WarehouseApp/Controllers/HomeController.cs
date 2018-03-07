using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using System;
using System.Net.Http;
using System.Web.Mvc;

namespace WarehouseApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            var telemetryClient = new TelemetryClient();
            var random = new Random();

            ViewBag.Message = "Your application description page.";

            telemetryClient.TrackEvent("About event");

            telemetryClient.TrackMetric(
                new MetricTelemetry()
                {
                    Name = "customnumber1",
                    Sum = random.Next(3, 10),
                    Count = 1
                });


            return View();
        }

        public ActionResult Contact()
        {
            var telemetryClient = new TelemetryClient();
            using (var op = telemetryClient.StartOperation<RequestTelemetry>("Contact"))
            {
                telemetryClient.TrackTrace("Before request");

                using (var client = new HttpClient())
                {
                    client.GetStringAsync("http://bing.com");
                }

                telemetryClient.TrackTrace("After request");
            }

            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}