using System;
using Metrics.Reports;
using System.Text;
using System.Runtime.Remoting.Messaging;
using System.Collections.Generic;

namespace Metrics.NET.InfluxDB
{
    public static class ConfigExtensions
    {
        /// <summary>
        /// Push metrics into InfluxDB 0.9+
        /// </summary>
        public static MetricsReports WithInflux (this MetricsReports reports, string host, int port, string user, string pass, string database, TimeSpan interval, ConfigOptions config = null)
        {
            var uri = string.Format (@"{0}://{1}:{2}/write?db={3}", config != null && config.UseHttps ? "https" : "http", host, port, database);
            if (config != null && !string.IsNullOrEmpty (config.UrlParams ()))
            {
                uri = uri + "&" + config.UrlParams ();
            }

            return reports.WithInflux (new Uri (uri), user, pass, interval);
        }

        /// <summary>
        /// Push metrics into InfluxDB 0.9+
        /// </summary>
        public static MetricsReports WithInflux (this MetricsReports reports, Uri influxdbUri, string username, string password, TimeSpan interval)
        {
            return reports.WithReport (new InfluxDbReport (influxdbUri, username, password), interval);
        }
    }

    public class ConfigOptions
    {
        public bool UseHttps { get; set; }
        public string RetentionPolicy { get; set; }
        public string Consistency { get; set; }

        internal string UrlParams()
        {
            var parameters = new List<string>();

            if (!string.IsNullOrEmpty (RetentionPolicy)){
                parameters.Add ("rp="+RetentionPolicy);                
            }

            if (!string.IsNullOrEmpty (Consistency)){
                parameters.Add ("consistency="+Consistency);                
            }

            return string.Join ("&", parameters);
        }
    }
}
