using System.Threading.Tasks;
using System.Web.Http;
using System.Collections.Generic;
using System.Net.Http;

namespace ApplicationInsightsBestPractices.WebApi.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public async Task<IEnumerable<string>> Get()
        {
            // This gets correlated to the request and other operations
            TelemetryClientWrapper.Instance.TrackEvent("SimpleCustomEvent");

            // Issue 1: This dependency does not get correlated to the request and other operations (becuase it is called asynchronously)
            await (new HttpClient()).GetAsync("http://www.bing.com");

            // Issue 2: This custom event and exception don't get correlated to the request and other operations (becuase they run
            // on another thread in the background)
            Task.Run(async () =>
            {
                TelemetryClientWrapper.Instance.TrackEvent("SampleCustomEventOnBackgroundThread");
                TelemetryClientWrapper.Instance.TrackException(new System.InvalidOperationException("Background Thread Exception"));
            });

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public async Task<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
