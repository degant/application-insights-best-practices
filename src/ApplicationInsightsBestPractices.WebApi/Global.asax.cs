using System.Web.Http;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsightsBestPractices.WebApi
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Adding below lines to correlate all dependenceis as well as any telemetry being logged on background threads 
            // such as custom events, exceptions etc.
            var filter = new ApplicationInsightsCorrelationActionFilterAttribute();
            GlobalConfiguration.Configuration.Filters.Add(filter);
            TelemetryConfiguration.Active.TelemetryInitializers.Add(filter);
        }
    }
}
