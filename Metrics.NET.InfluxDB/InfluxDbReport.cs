using System;
using Metrics.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Metrics.NET.InfluxDB
{
    internal class InfluxDbReport : BaseReport
    {
        private readonly InfluxDbHttpTransport _transport;
        private List<InfluxDbRecord> _data;
        private ConfigOptions _config;

        internal InfluxDbReport (Uri influxdb, string username, string password, ConfigOptions config)
        {
            this._transport = new InfluxDbHttpTransport (influxdb, username, password, config);
            this._config = config;
        }

        private void Pack (string name, object value, MetricTags tags)
        {
            this._data.Add (new InfluxDbRecord (name, value, tags, _config));
        }

        private void Pack (string name, IEnumerable<string> columns, IEnumerable<object> values, MetricTags tags)
        {
            this._data.Add (new InfluxDbRecord (name, columns, values, tags, _config));
        }

        protected override void StartReport (string contextName)
        {
            this._data = new List<InfluxDbRecord> ();
            base.StartReport (contextName);
        }

        protected override void EndReport (string contextName)
        {
            base.EndReport (contextName);
            this._transport.Send (_data);
            this._data = null;
        }

        protected override void ReportGauge (string name, double value, Unit unit, MetricTags tags)
        {
            if (!double.IsNaN (value) && !double.IsInfinity (value)) {
                Pack (name, value, tags);
            }
        }

        protected override void ReportCounter (string name, MetricData.CounterValue value, Unit unit, MetricTags tags)
        {
            if (!value.Items.Any ()) {
                Pack (name, value.Count, tags);
                return;
            }

            var cols = new List<string> (new [] { "total" });
            cols.AddRange (value.Items.Select (x => x.Item));

            var data = new List<object> (new object[] { value.Count });
            data.AddRange (value.Items.Select (x => (object)x.Count));

            Pack (name, cols, data, tags);
        }

        protected override void ReportMeter (string name, MetricData.MeterValue value, Unit unit, TimeUnit rateUnit, MetricTags tags)
        {
            var cols = new [] { "count", "rate1m", "rate5m", "rate15m" };
            var data = new object[] { value.Count, value.OneMinuteRate, value.FiveMinuteRate, value.FifteenMinuteRate };

            Pack (name, cols, data, tags);
        }

        protected override void ReportHistogram (string name, MetricData.HistogramValue value, Unit unit, MetricTags tags)
        {
            var cols = new [] { "count", "last", "samples" }.ToList ();
            var data = new object[] { value.Count, value.LastValue, value.SampleSize }.ToList ();
        
            if (value.LastUserValue != null) {
                cols.Add ("user.last");
                data.Add (value.LastUserValue);
            }

            if (value.MinUserValue != null) {
                cols.Add ("user.min");
                data.Add (value.MinUserValue);
            }

            if (value.MaxUserValue != null) {
                cols.Add ("user.max");
                data.Add (value.MaxUserValue);            
            }

            Pack (name, cols, data, tags);
        }

        protected override void ReportTimer (string name, MetricData.TimerValue value, Unit unit, TimeUnit rateUnit, TimeUnit durationUnit, MetricTags tags)
        {
            var cols = new [] { "count", "sessions", "rate1m", "rate5m", "rate15m", "samples" }.ToList ();
            var data = new object[] { value.Rate.Count, value.ActiveSessions, value.Rate.OneMinuteRate, value.Rate.FiveMinuteRate, value.Rate.FifteenMinuteRate, value.Histogram.SampleSize }.ToList ();

            if (value.Histogram.LastUserValue != null) {
                cols.Add ("user.last");
                data.Add (value.Histogram.LastUserValue);
            }

            if (value.Histogram.MinUserValue != null) {
                cols.Add ("user.min");
                data.Add (value.Histogram.MinUserValue);
            }

            if (value.Histogram.MaxUserValue != null) {
                cols.Add ("user.max");
                data.Add (value.Histogram.MaxUserValue);
            }

            Pack (name, cols, data, tags);
        }

        protected override void ReportHealth (HealthStatus status)
        {
        }
    }
}
