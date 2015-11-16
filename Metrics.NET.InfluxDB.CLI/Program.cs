using System;
using CLAP;
using System.Threading;

namespace Metrics.NET.InfluxDB.CLI
{
    class Program
    {
        public static void Main (string[] args)
        {
            Parser.RunConsole<Program>(args);
        }

        [Verb(IsDefault = true)]
        static void PushData(
            [DefaultValue("localhost")]string host, 
            [DefaultValue(8086)]int port, 
            [DefaultValue("admin")]string user, 
            [DefaultValue("admin")]string pass, 
            [DefaultValue("test")]string db, 
            [DefaultValue(5)]int influxFreq,
            [DefaultValue(5)]int consoleFreq)
        {
            Metric.Config.WithReporting(report => {
                report.WithInflux(host, port, user, pass, db, TimeSpan.FromSeconds(influxFreq));
                report.WithConsoleReport(TimeSpan.FromSeconds(consoleFreq));
            });

            var rand = new Random (Guid.NewGuid ().GetHashCode ());

            while (true) {
                Metric.Counter ("my.counter", Unit.Events).Increment ();
                Metric.Meter ("my.meter", Unit.Events).Mark (rand.Next (0, 1000));
                Metric.Gauge ("my.gauge", rand.NextDouble, Unit.Custom ("cpu%"));
                Thread.Sleep (rand.Next (500, 3000));
            }
        }
    }
}
