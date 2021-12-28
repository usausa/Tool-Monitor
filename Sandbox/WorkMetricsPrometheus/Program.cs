namespace WorkMetricsPrometheus;

using System.Diagnostics.Metrics;
using System.Threading;

using OpenTelemetry;
using OpenTelemetry.Metrics;

public static class Program
{
    private static readonly Meter Meter = new("Work.Metrics");
    private static readonly Counter<int> TestCounter = Meter.CreateCounter<int>("test-counter");

    public static void Main()
    {
        using var meterProvider = Sdk.CreateMeterProviderBuilder()
            .AddMeter("Work.Metrics")
            .AddPrometheusExporter(opt =>
            {
                opt.StartHttpListener = true;
                opt.HttpListenerPrefixes = new[] { "http://localhost:9184/" };
            })
            .Build();

        var rand = new Random();
        while (!Console.KeyAvailable)
        {
            Thread.Sleep(1000);
            TestCounter.Add(rand.Next(10));
        }
    }
}
