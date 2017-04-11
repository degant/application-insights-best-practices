using System.Net.Http;
using System.Threading.Tasks;

namespace ApplicationInsightsBestPractices.WebApi.Controllers
{
    internal class FooBar
    {
        internal static async Task AsyncTask()
        {
            // This gets correlated to the request and other operations
            TelemetryClientWrapper.Instance.TrackEvent("SampleCustomEventInternal");

            HttpClient hc = new HttpClient();
            
            // This dependency does not get correlated to the request and other operations (becuase it is called asynchronously)
            await hc.GetAsync("http://www.bing.com");
        }
    }
}