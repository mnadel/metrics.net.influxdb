using System;
using Metrics.Reports;
using System.Collections.Generic;

namespace Metrics.NET.InfluxDB
{
    /// <summary>
    /// Configure Metrics.NET to report data to InfluxDB 0.9+
    /// </summary>
    public static class ConfigExtensions
    {
        /// <summary>
        /// Push metrics into InfluxDB 0.9+
        /// </summary>
        public static MetricsReports WithInflux (this MetricsReports reports, string host, int port, string user, string pass, string database, TimeSpan interval, ConfigOptions config = null)
        {
            var cfg = config ?? new ConfigOptions ();

            var uri = cfg.BuildUri (host, port, database);

            return reports.WithInflux (uri, user, pass, interval, cfg);
        }

        /// <summary>
        /// Push metrics into InfluxDB 0.9+
        /// </summary>
        public static MetricsReports WithInflux (this MetricsReports reports, Uri influxdbUri, string username, string password, TimeSpan interval, ConfigOptions config = null)
        {
            return reports.WithReport (new InfluxDbReport (influxdbUri, username, password, config ?? new ConfigOptions ()), interval);
        }
    }

    /// <summary>
    /// Additional configuration options
    /// </summary>
    public class ConfigOptions
    {
        /// <summary>
        /// Set whether or not to use SSL when posting data to InfluxDB
        /// </summary>
        /// <value><c>true</c> if use https; otherwise, <c>false</c>.</value>
        public bool UseHttps { get; set; }

        /// <summary>
        /// Set the target retention policy for the write
        /// </summary>
        /// <value>The retention policy.</value>
        public string RetentionPolicy { get; set; }

        /// <summary>
        /// Set the number of nodes that must confirm the write
        /// </summary>
        /// <value>One of: one,quorum,all,any</value>
        public string Consistency { get; set; }

        /// <summary>
        /// Specify the acceptable error rate before a curcuit is 
        /// tripped which will temporarily prevent writing data to 
        /// InfluxDB, in the form of: EventCount / TimeSpan, eg: 3 / 00:00:30
        /// </summary>
        /// <value></value>
        public string BreakerRate { get; set; }

        /// <summary>
        /// Gets or sets the http timeout in milliseconds
        /// </summary>
        /// <value>The http timeout.</value>
        public int HttpTimeoutMillis { get; set; }

        /// <summary>
        /// Instantiate a new config object
        /// </summary>
        public ConfigOptions()
        {
            BreakerRate = "3 / 00:00:30";
            HttpTimeoutMillis = 5000;
        }

        internal Uri BuildUri(string host, int port, string database)
        {
            var parameters = new List<string> 
            {
                "db="+database    
            };

            if (!string.IsNullOrEmpty (RetentionPolicy)) {
                parameters.Add ("rp="+RetentionPolicy);                
            }

            if (!string.IsNullOrEmpty (Consistency)) {
                parameters.Add ("consistency="+Consistency);                
            }

            var queryStringParams = string.Join ("&", parameters);

            return new Uri(string.Format (@"{0}://{1}:{2}/write?{3}", UseHttps ? "https" : "http", host, port, queryStringParams));
        }
    }
}
