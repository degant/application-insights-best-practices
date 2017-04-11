using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Threading;

using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace ApplicationInsightsBestPractices.WebApi
{
    /// <summary>
    /// The Correlation Action Filter Attribute helps correlate dependencies and other asynchronous events (such as those running on background threads)
    /// for example a custom event or an exception on a background thread. The operation ID on these is manually set by using the ITelemetryInitializer
    /// interface. Please note that this filter can be used for fixing the AI correlation issues in Web API only (i.e System.Web.Http.Filters) and will 
    /// not work for a MVC Web application (i.e System.Web.Mvc)
    /// </summary>
    public class ApplicationInsightsCorrelationActionFilterAttribute : System.Web.Http.Filters.ActionFilterAttribute, ITelemetryInitializer
    {
        private static AsyncLocal<RequestTelemetry> currentRequestTelemetry = new AsyncLocal<RequestTelemetry>();

        /// <summary>
        /// Saves the request telemetry so that it can be used later
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var request = HttpContext.Current.GetRequestTelemetry();
            currentRequestTelemetry.Value = request;

            base.OnActionExecuting(actionContext);
        }

        /// <summary>
        /// Deletes the request telemetry
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            currentRequestTelemetry.Value = null;

            base.OnActionExecuted(actionExecutedContext);
        }

        /// <summary>
        /// Sets all operation details so that the current telemetry event correlates back to the parent request operation. The fields
        /// are only set if they are null so essentially this will only be used for asynchronous dependency calls and other telemetry
        /// logged on background threads.
        /// </summary>
        /// <param name="telemetry"></param>
        public void Initialize(ITelemetry telemetry)
        {
            var request = currentRequestTelemetry.Value;

            if (request == null)
                return;

            if (string.IsNullOrEmpty(telemetry.Context.Operation.Id) && !string.IsNullOrEmpty(request.Context.Operation.Id))
            {
                telemetry.Context.Operation.Id = request.Context.Operation.Id;
            }

            if (string.IsNullOrEmpty(telemetry.Context.Operation.ParentId) && !string.IsNullOrEmpty(request.Id))
            {
                telemetry.Context.Operation.ParentId = request.Id;
            }

            if (string.IsNullOrEmpty(telemetry.Context.Operation.Name) && !string.IsNullOrEmpty(request.Name))
            {
                telemetry.Context.Operation.Name = request.Name;
            }

            if (string.IsNullOrEmpty(telemetry.Context.User.Id) && !string.IsNullOrEmpty(request.Context.User.Id))
            {
                telemetry.Context.User.Id = request.Context.User.Id;
            }

            if (string.IsNullOrEmpty(telemetry.Context.Session.Id) && !string.IsNullOrEmpty(request.Context.Session.Id))
            {
                telemetry.Context.Session.Id = request.Context.Session.Id;
            }
        }
    }
}