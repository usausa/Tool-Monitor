namespace WorkMetricsObserver;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Threading;

public static class Program
{
    private static readonly Meter Meter = new("Work.Metrics");
    private static int gauge;

    public static void Main()
    {
        Meter.CreateObservableGauge<int>("test-gauge", () =>
        {
            Debug.WriteLine("Pull value");
            return gauge;
        });

        var rand = new Random();
        while (!Console.KeyAvailable)
        {
            Thread.Sleep(1000);
            gauge = rand.Next(10);
        }
    }
}
