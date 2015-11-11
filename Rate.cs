using System;
using Polly;

namespace Metrics.NET.InfluxDB
{
    public class Rate
    {
        public int Events { get; private set; }

        public TimeSpan Period { get; private set; }

        public Rate (int events, TimeSpan period)
        {
            Events = events;
            Period = period;
        }

        /// <summary>
        /// Rate specification in the form of: <code>events / timeframe</code>
        /// </summary>
        public Rate (string specification)
        {
            var parts = specification.Split ('/');
            Events = int.Parse (parts [0].Trim ());
            Period = TimeSpan.Parse (parts [1].Trim ());
        }

        public Policy AsPolicy ()
        {
            return Policy.Handle<Exception> ().CircuitBreaker (Events, Period);
        }
    }
}
