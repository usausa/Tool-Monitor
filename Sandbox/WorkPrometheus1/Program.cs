using Prometheus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

app.MapGet("/", () =>
{
    ApplicationMetrics.IncrementHello();
    return "Hello";
});

app.MapMetrics("/metrics", null);

app.Run();

public static class ApplicationMetrics
{
    private static readonly Counter HelloCounter =
        Metrics.CreateCounter("call_hello", "Call hello count.");

    public static void IncrementHello() => HelloCounter.Inc();

    private static readonly Gauge TestGauge =
        Metrics.CreateGauge("test_value", "Test value.");

    public static void SetTest(double value) => TestGauge.Set(value);
}
