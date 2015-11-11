using System.Text;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace Metrics.NET.InfluxDB
{
    internal class InfluxDbRecord
    {
        public string LineProtocol { get; private set; }

        internal InfluxDbRecord (string name, IEnumerable<string> columns, IEnumerable<object> data, MetricTags tags)
        {
            // see: https://influxdb.com/docs/v0.9/write_protocols/write_syntax.html

            var record = new StringBuilder ();
            record.Append (Escape (name));

            var allTags = GetAllTags (tags);
            if (allTags.Any ()) 
            {
                record.Append (",");
                record.Append (string.Join(",", allTags.Select (t => string.Format ("{0}={1}", Escape(t.Item1), Escape(t.Item2)))));
            }

            record.Append (" ");

            var fieldKeypairs = new List<string> ();

            foreach (var pair in columns.Zip(data, (col, dat) => new { col, dat })) {
                fieldKeypairs.Add (string.Format ("{0}={1}", Escape (pair.col), pair.dat));
            }

            record.Append (string.Join (",", fieldKeypairs));

            LineProtocol = record.ToString ();
        }

        internal IEnumerable<Tuple<string, string>> GetAllTags(MetricTags tags)
        {
            var allTags = new List<Tuple<string, string>> 
            {
                new Tuple<string, string>("host", Environment.MachineName),
                new Tuple<string, string>("user", Environment.UserName),
                new Tuple<string, string>("pid", Process.GetCurrentProcess ().Id.ToString ()),
            };

            if (tags.Tags != null && tags.Tags.Length > 0) 
            {
                allTags.AddRange (tags.Tags.Select (t => new Tuple<string, string>(Escape(t), "true")));
            }

            return allTags;
        }

        private static string Escape (string v)
        {
            return v.Replace (" ", "\\ ").Replace (",", "\\,").Replace ("=", "\\=");
        }
    }
}

