| Travis CI | Appveyor |
|----------|----------|
|[![Travis CI Status](https://travis-ci.org/mnadel/metrics.net.influxdb.svg?branch=master)](https://travis-ci.org/mnadel/metrics.net.influxdb)|[![Appveyor Status](https://ci.appveyor.com/api/projects/status/gkd1n41vkigtm4q4?svg=true)](hhttps://ci.appveyor.com/project/mnadel/metrics-net-influxdb)|

# Download

Available on [NuGet.org](https://www.nuget.org/packages/Metrics.NET.InfluxDB/) as [Metrics.NET.InfluxDB](https://www.nuget.org/packages/Metrics.NET.InfluxDB/).

# Usage

Nearly identical to the usage shown in the [Metrics.NET Wiki](https://github.com/etishor/Metrics.NET/wiki/InfluxDb), but using `WithInflux` instead of `WithInfluxDb`:

    Metric.Config.WithReporting(report => 
        report.WithInflux("192.168.1.8", 8086, "admin", "admin", "metrics", TimeSpan.FromSeconds(5))