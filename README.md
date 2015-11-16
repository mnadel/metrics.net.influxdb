| Travis CI | AppVeyor |
|----------|----------|
|[![Travis CI Status](https://travis-ci.org/mnadel/metrics.net.influxdb.svg?branch=master)](https://travis-ci.org/mnadel/metrics.net.influxdb)|[![AppVeyor Status](https://ci.appveyor.com/api/projects/status/gkd1n41vkigtm4q4?svg=true)](https://ci.appveyor.com/project/mnadel/metrics-net-influxdb)|

# Download

Available on [NuGet.org](https://www.nuget.org/packages/Metrics.NET.InfluxDB/) as [Metrics.NET.InfluxDB](https://www.nuget.org/packages/Metrics.NET.InfluxDB/).

# Usage

Nearly identical to the usage shown in the [Metrics.NET Wiki](https://github.com/etishor/Metrics.NET/wiki/InfluxDb), but using `WithInflux` instead of `WithInfluxDb`:

    Metric.Config.WithReporting(report => 
        report.WithInflux("192.168.1.8", 8086, "admin", "admin", "metrics", TimeSpan.FromSeconds(5))

# Additional Configs

Additional configurations are available via `Metrics.NET.InfluxDB.ConfigOptions`. Most are passed-through to InfluxDB. 

One noteworthy configuration affects the behavior of this Reporter: `BreakerRate`.

## BreakerRate

This configures a [circuit breaker](https://github.com/michael-wolfenden/Polly) that protects your InfluxDB endpoint. This configuration specifies the number of allowed errors, and a "cool-down" period where no additional requests will be made to the InfluxDB endpoint.

The rate is expressed as: `# of events / timeframe` where `# of events` is an integer, and `timeframe` is a string that can be parsed by `TimeSpan.Parse()`.

So, for example, `2 / 00:00:15` would trip the circuit if two errors occur, and then will wait fifteen seconds before allowing additional requests to InfluxDB.