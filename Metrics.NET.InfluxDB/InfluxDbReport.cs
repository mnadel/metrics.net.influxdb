using System;
using Metrics.Reporters;
using System.Collections.Generic;
using System.Linq;

namespace Metrics.NET.InfluxDB
{
    internal class InfluxDbReport : BaseReport
    {
        private static readonly string[] GaugeColumns = { "Value" };

        private static readonly string[] CounterColumns = { "Count" };

        private static readonly string[] MeterColumns = {
            "Total Count",
            "Mean Rate",
            "1 Min Rate",
            "5 Min Rate",
            "15 Min Rate",
        };

        private static readonly string[] HistogramColumns = { 
            "Total Count",
            "Last",
            "Last User Value",
            "Min",
            "Min User Value",
            "Mean",
            "Max",
            "Max User Value",
            "StdDev",
            "Median",
            "Percentile 75%",
            "Percentile 95%",
            "Percentile 98%",
            "Percentile 99%",
            "Percentile 99.9%",
            "Sample Size"
        };

        private static readonly string[] TimerColumns = {
            "Total Count", "Active Sessions",
            "Mean Rate", "1 Min Rate", "5 Min Rate", "15 Min Rate",
            "Last", "Last User Value", "Min", "Min User Value", "Mean", "Max", "Max User Value",
            "StdDev", "Median", "Percentile 75%", "Percentile 95%", "Percentile 98%", "Percentile 99%", "Percentile 99.9%", "Sample Size" 
        };

        private readonly InfluxDbHttpTransport _transport;
        private List<InfluxDbRecord> _data;

        internal InfluxDbReport (Uri influxdb, string username, string password, ConfigOptions config)
        {
            this._transport = new InfluxDbHttpTransport (influxdb, username, password, config);
        }

        private void Pack (string name, IEnumerable<string> columns, object value, MetricTags tags)
        {
            Pack (name, columns, Enumerable.Repeat (value, 1), tags);
        }

        private void Pack (string name, IEnumerable<string> columns, IEnumerable<object> values, MetricTags tags)
        {
            this._data.Add (new InfluxDbRecord (name, columns, values, tags));
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
                Pack (name, GaugeColumns, value, tags);
            }
        }

        protected override void ReportCounter (string name, MetricData.CounterValue value, Unit unit, MetricTags tags)
        {
            var itemColumns = value.Items.SelectMany (i => new[] { i.Item + " - Count", i.Item + " - Percent" });
            var columns = CounterColumns.Concat (itemColumns);

            var itemValues = value.Items.SelectMany (i => new[] { i.Count, i.Percent });

            foreach (var data in new[] {(double) value.Count}.Concat(itemValues)) {
                Pack (name, columns, data, tags);
            }
        }

        protected override void ReportMeter (string name, MetricData.MeterValue value, Unit unit, TimeUnit rateUnit, MetricTags tags)
        {
            var itemColumns = value.Items.SelectMany (i => new[] { 
                i.Item + " - Count", 
                i.Item + " - Percent", 
                i.Item + " - Mean Rate",
                i.Item + " - 1 Min Rate",
                i.Item + " - 5 Min Rate",
                i.Item + " - 15 Min Rate"
            });
            var columns = MeterColumns.Concat (itemColumns);

            var itemValues = value.Items.SelectMany (i => new[] {
                i.Value.Count, 
                i.Percent,
                i.Value.MeanRate, 
                i.Value.OneMinuteRate, 
                i.Value.FiveMinuteRate, 
                i.Value.FifteenMinuteRate
            });

            var datas = new[] {
                value.Count,
                value.MeanRate,
                value.OneMinuteRate,
                value.FiveMinuteRate,
                value.FifteenMinuteRate
            }.Concat (itemValues);

            foreach (var data in datas) {
                Pack (name, columns, data, tags);
            }
        }

        protected override void ReportHistogram (string name, MetricData.HistogramValue value, Unit unit, MetricTags tags)
        {
            Pack (name, HistogramColumns, new object[] {
                value.Count,
                value.LastValue,
                value.LastUserValue ?? "\"\"",
                value.Min,
                value.MinUserValue ?? "\"\"",
                value.Mean,
                value.Max,
                value.MaxUserValue ?? "\"\"",
                value.StdDev,
                value.Median,
                value.Percentile75,
                value.Percentile95,
                value.Percentile98,
                value.Percentile99,
                value.Percentile999,
                value.SampleSize
            }, tags);
        }

        protected override void ReportTimer (string name, MetricData.TimerValue value, Unit unit, TimeUnit rateUnit, TimeUnit durationUnit, MetricTags tags)
        {
            Pack (name, TimerColumns, new object[] {

                value.Rate.Count,
                value.ActiveSessions,
                value.Rate.MeanRate,
                value.Rate.OneMinuteRate,
                value.Rate.FiveMinuteRate,
                value.Rate.FifteenMinuteRate,
                value.Histogram.LastValue,
                value.Histogram.LastUserValue ?? "\"\"",
                value.Histogram.Min,
                value.Histogram.MinUserValue ?? "\"\"",
                value.Histogram.Mean,
                value.Histogram.Max,
                value.Histogram.MaxUserValue ?? "\"\"",
                value.Histogram.StdDev,
                value.Histogram.Median,
                value.Histogram.Percentile75,
                value.Histogram.Percentile95,
                value.Histogram.Percentile98,
                value.Histogram.Percentile99,
                value.Histogram.Percentile999,
                value.Histogram.SampleSize
            }, tags);
        }

        protected override void ReportHealth (HealthStatus status)
        {
        }
    }
}
