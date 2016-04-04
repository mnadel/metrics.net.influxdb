using System;
using Metrics.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Metrics.NET.InfluxDB
{
    internal class InfluxDbReport : BaseReport
    {
        private readonly InfluxDbHttpTransport _transport;
        private readonly ConfigOptions _config;
        private List<InfluxDbRecord> _data;

        internal InfluxDbReport(Uri influxdb, string username, string password, ConfigOptions config)
        {
            _transport = new InfluxDbHttpTransport(influxdb, username, password, config);
            _config = config;
        }

        private void Pack(string name, object value, MetricTags tags)
        {
            _data.Add(new InfluxDbRecord(name, value, tags, _config));
        }

        private void Pack(string name, IEnumerable<string> columns, IEnumerable<object> values, MetricTags tags)
        {
            _data.Add(new InfluxDbRecord(name, columns, values, tags, _config));
        }

        protected override void StartReport(string contextName)
        {
            _data = new List<InfluxDbRecord>();
            base.StartReport(contextName);
        }

        protected override void EndReport(string contextName)
        {
            base.EndReport(contextName);
            _transport.Send(_data);
            _data = null;
        }

        protected override void ReportGauge(string name, double value, Unit unit, MetricTags tags)
        {
            if (!double.IsNaN(value) && !double.IsInfinity(value))
            {
                Pack(name, value, tags);
            }
        }

        protected override void ReportCounter(string name, MetricData.CounterValue value, Unit unit, MetricTags tags)
        {
            if (!value.Items.Any())
            {
                Pack(name, value.Count, tags);
                return;
            }

            var cols = new List<string>(new[] { "total" });
            cols.AddRange(value.Items.Select(x => x.Item));

            var data = new List<object>(new object[] { value.Count });
            data.AddRange(value.Items.Select(x => (object)x.Count));

            Pack(name, cols, data, tags);
        }

        protected override void ReportMeter(string name, MetricData.MeterValue value, Unit unit, TimeUnit rateUnit, MetricTags tags)
        {
            var data = new Dictionary<string, object>();

            AddMeterValues(data, value);

            var keys = data.Keys;
            var values = data.Keys.Select(k => data[k]);

            Pack(name, keys, values, tags);
        }

        protected override void ReportHistogram(string name, MetricData.HistogramValue value, Unit unit, MetricTags tags)
        {
            var data = new Dictionary<string, object>
            {
                {"count", value.Count}, 
                {"last", value.LastValue}, 
                {"samples", value.SampleSize},
            };

            AddHistogramValues(data, value);

            var keys = data.Keys;
            var values = data.Keys.Select(k => data[k]);

            Pack(name, keys, values, tags);
        }

        protected override void ReportTimer(string name, MetricData.TimerValue value, Unit unit, TimeUnit rateUnit, TimeUnit durationUnit, MetricTags tags)
        {
            var data = new Dictionary<string, object>();

            AddMeterValues(data, value.Rate);
            AddHistogramValues(data, value.Histogram);

            var keys = data.Keys;
            var values = data.Keys.Select(k => data[k]);

            Pack(name, keys, values, tags);
        }

        private void AddMeterValues(IDictionary<string, object> values, MetricData.MeterValue meter)
        {
            values.Add("count", meter.Count);

            values.Add("rate1m", meter.OneMinuteRate);
            values.Add("rate5m", meter.FiveMinuteRate);
            values.Add("rate15m", meter.FifteenMinuteRate);
            values.Add("rateMean", meter.MeanRate);
        }

        private static void AddHistogramValues(IDictionary<string, object> values, MetricData.HistogramValue hist)
        {
            values.Add("samples", hist.SampleSize);
            values.Add("count", hist.Count);
            values.Add("min", hist.Min);
            values.Add("max", hist.Max);
            values.Add("mean", hist.Mean);
            values.Add("median", hist.Median);
            values.Add("stddev", hist.StdDev);
            values.Add("p999", hist.Percentile999);
            values.Add("p99", hist.Percentile99);
            values.Add("p99", hist.Percentile98);
            values.Add("p95", hist.Percentile95);
            values.Add("p75", hist.Percentile75);

            if (hist.LastUserValue != null)
            {
                values.Add("user.last", hist.LastUserValue);
            }

            if (hist.MinUserValue != null)
            {
                values.Add("user.min", hist.MinUserValue);
            }

            if (hist.MaxUserValue != null)
            {
                values.Add("user.max", hist.MaxUserValue);
            }
        }

        protected override void ReportHealth(HealthStatus status)
        {
        }
    }
}
