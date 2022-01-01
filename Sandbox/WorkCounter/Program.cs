namespace WorkCounter;

using System.Diagnostics;
using System.Runtime.Versioning;

[SupportedOSPlatform("windows")]
public static class Program
{
    public static void Main()
    {
        // CPU Usage, GAUGE, LINE1, -l 0 -u 100
        Dump("cpu", Prepare("Processor", "% Processor Time", "_Total"));
        // Memory Usage, GAUGE, AREA, -l 0 -u 100
        Dump("memory", Prepare("Memory", "% Committed Bytes In Use"));
        // Uptime, GAUGE, AREA, -b 1000 -l 0
        Dump("uptime", Prepare("System", "System Up Time"), Single.Parse("1.1574074074074073e-005"));
    }

    private static void Dump(string name, PerformanceCounter[] counters, float? multiply = null)
    {
        Debug.WriteLine("----" + name);
        foreach (var counter in counters)
        {
            Debug.WriteLine(multiply.HasValue ? counter.NextValue() * multiply : counter.NextValue());
        }
    }

    // TODO 一度ダミーNextValue()の必要あり

    private static PerformanceCounter[] Prepare(string categoryName, string counterName, string? instanceName = null)
    {
        var counters = Create(categoryName, counterName, instanceName);
        foreach (var counter in counters)
        {
            counter.NextValue();
        }
        return counters;
    }

    private static PerformanceCounter[] Create(string categoryName, string counterName, string? instanceName = null)
    {
        if (!String.IsNullOrEmpty(instanceName))
        {
            return new[] { new PerformanceCounter(categoryName, counterName, instanceName) };
        }

        var pcc = new PerformanceCounterCategory(categoryName);
        if (pcc.CategoryType == PerformanceCounterCategoryType.SingleInstance)
        {
            return new[] { new PerformanceCounter(categoryName, counterName) };
        }

        var instanceNames = pcc.GetInstanceNames().OrderBy(x => x).ToArray();
        return instanceNames.Select(x => new PerformanceCounter(categoryName, counterName, x)).ToArray();
    }
}
