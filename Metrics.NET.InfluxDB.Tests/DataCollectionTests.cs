using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Metrics.NET.InfluxDB.Tests
{
    [TestFixture]
    public class DataCollectionTests
    {
        [Test]
        public void CanCollectMetrics()
        {
            Metric.Timer("test", Unit.Calls).NewContext("user-value").Dispose();

            var dict = new Dictionary<string, object>();

            var data = Metric.Context(null).DataProvider.CurrentMetricsData;

            var timer = data.Timers.First();
            timer.Value.Histogram.AddHistogramValues(dict);
            timer.Value.Rate.AddMeterValues(dict);
        }
    }
}
