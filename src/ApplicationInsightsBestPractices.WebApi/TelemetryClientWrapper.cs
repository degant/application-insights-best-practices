using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;

namespace ApplicationInsightsBestPractices.WebApi
{
    /// <summary>
    /// A singleton Telemetry Client Wrapper for Application Insights. This is because AI team recommends leveraging a single instance of
    /// the TelemetryClient for every module. This helps avoid the heavy operation of creating a TelemetryClient every time.
    /// </summary>
    public sealed class TelemetryClientWrapper
    {
        #region Singleton
        private static readonly Lazy<TelemetryClientWrapper> lazy =
            new Lazy<TelemetryClientWrapper>(() => new TelemetryClientWrapper());

        public static TelemetryClientWrapper Instance { get { return lazy.Value; } }

        private TelemetryClientWrapper()
        {
            telemetryClient = new TelemetryClient();
        }
        #endregion

        private readonly TelemetryClient telemetryClient;

        public void TrackEvent(string eventName, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            telemetryClient.TrackEvent(eventName, properties, metrics);
        }

        public void TrackException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            telemetryClient.TrackException(exception, properties, metrics);
        }

        public void TrackHandledException(Exception exception, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            if (properties == null) properties = new Dictionary<string, string>();
            properties.Add(new KeyValuePair<string, string>("Handled", "true"));
            telemetryClient.TrackException(exception, properties, metrics);
        }
    }
}