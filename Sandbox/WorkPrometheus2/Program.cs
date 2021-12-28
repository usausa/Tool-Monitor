using System.Diagnostics.Metrics;

using OpenTelemetry.Metrics;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryMetrics(configure =>
{
    configure.AddAspNetCoreInstrumentation();
    configure.AddMeter("ApplicationMetrics");
    configure.AddPrometheusExporter();
    configure.AddConsoleExporter(options =>
    {
        options.MetricReaderType = MetricReaderType.Periodic;
        options.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds = 10000;
    });
});

// Add services to the container.

var app = builder.Build();

app.MapGet("/", () =>
{
    ApplicationMetrics.IncrementHello();
    return "Hello";
});

app.UseOpenTelemetryPrometheusScrapingEndpoint();

app.Run();

public static class ApplicationMetrics
{
    private static readonly Meter Meter = new("ApplicationMetrics");

    private static readonly Counter<int> HelloCounter = Meter.CreateCounter<int>("call_hello");

    public static void IncrementHello() => HelloCounter.Add(1);
}
