namespace WorkMetricsSimple;

using System.Diagnostics.Metrics;
using System.Threading;

public static class Program
{
    private static readonly Meter Meter = new("Work.Metrics");
    private static readonly Counter<int> TestCounter = Meter.CreateCounter<int>("test-counter");

    public static void Main()
    {
        var rand = new Random();
        while (!Console.KeyAvailable)
        {
            Thread.Sleep(1000);
            TestCounter.Add(rand.Next(10));
        }
    }
}
