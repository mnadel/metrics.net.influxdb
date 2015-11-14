using System;
using System.Net.Http;
using Polly;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http.Headers;

namespace Metrics.NET.InfluxDB
{
    internal class InfluxDbHttpTransport
    {
        private readonly HttpClient _client;
        private readonly Policy _policy;
        private readonly Uri _uri;

        internal InfluxDbHttpTransport (Uri uri, string username, string password, string breakerRate)
        {
            _uri = uri;

            var byteArray = Encoding.ASCII.GetBytes (string.Format ("{0}:{1}", username, password));

            _client = new HttpClient () {
                DefaultRequestHeaders = {
                    Authorization = new AuthenticationHeaderValue ("Basic", Convert.ToBase64String (byteArray))
                }
            };

            _policy = new Rate (breakerRate).AsPolicy ();
        }

        internal void Send (IEnumerable<InfluxDbRecord> records)
        {
            using (Metric.Context ("Metrics.NET").Timer ("influxdb.report.timer", Unit.Calls).NewContext ()) {
                _policy.Execute (() => {
                    var content = string.Join ("\n", records.Select (d => d.LineProtocol));

                    var task = _client.PostAsync (_uri, new StringContent (content));

                    task.ContinueWith (m => {
                        if ((int)m.Result.StatusCode == 204) {
                            Metric.Context ("Metrics.NET").Counter ("influxdb.success.count", Unit.Events).Increment ();
                        } else {
                            Metric.Context ("Metrics.NET").Counter ("influxdb.fail.count", Unit.Events).Increment ();
                            var response = m.Result.Content.ReadAsStringAsync ().Result;
                            throw new Exception (string.Format ("Error posting to [{0}] {1} {2}", _uri, m.Result.StatusCode, response)); 
                        }
                    });
                });
            }
        }
    }
}
