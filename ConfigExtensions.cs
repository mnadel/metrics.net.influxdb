using System;
using Metrics.Reports;

namespace Metrics.NET.InfluxDB
{
    public static class ConfigExtensions
    {
        /// <summary>
        /// Push metrics into InfluxDB 0.9+
        /// </summary>
        public static MetricsReports WithInflux (this MetricsReports reports, string host, int port, string user, string pass, string database, TimeSpan interval, bool useHttps = false)
        {
            return reports.WithInflux (new Uri (string.Format (@"{0}://{1}:{2}/write?db={3}", useHttps ? "https" : "http", host, port, database)), user, pass, interval);
        }

        /// <summary>
        /// Push metrics into InfluxDB 0.9+
        /// </summary>
        public static MetricsReports WithInflux (this MetricsReports reports, Uri influxdbUri, string username, string password, TimeSpan interval)
        {
            return reports.WithReport (new InfluxDbReport (influxdbUri, username, password), interval);
        }
    }
}
