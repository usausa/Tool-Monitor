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
        // Disk Usage(逆)
        Dump("disk", Prepare("LogicalDisk", "% Free Space"));
        // Uptime, GAUGE, AREA, -b 1000 -l 0
        Dump("uptime", Prepare("System", "System Up Time"), Single.Parse("1.1574074074074073e-005"));
        // Processor Queue Length, system, --base 1000 -l 0, -base 1000 -l 0, LINE
        Dump("processorqueue", Prepare("System", "Processor Queue Length"));
        // Disk Time, disk, --base 1000 -l 0, LINE
        Dump("disktime", Prepare("LogicalDisk", "% Disk Time"));
        // Disk Queue Length, disk, --base 1000 -l 0, LINE
        Dump("diskqueue", Prepare("PhysicalDisk", "Current Disk Queue Length"));
        // Ex
        Dump("memory_page", Prepare("Memory", "Pages/sec"));
        Dump("handle", Prepare("Process V2", "Handle Count", "_Total"));
        Dump("process", Prepare("System", "Processes"));
        Dump("thread", Prepare("Process V2", "Thread Count", "_Total"));
        Dump("tcp4_connections_established", Prepare("TCPv4", "Connections Established"));
        // TODO SQL
        // TODO IIS
        // Hyper-V
        Dump("vm_health_ok", Prepare("Hyper-V Virtual Machine Health Summary", "Health Ok"));
        Dump("vm_health_ng", Prepare("Hyper-V Virtual Machine Health Summary", "Health Critical"));
        Dump("vm_vid_partitions", Prepare("Hyper-V VM Vid Driver", "VidPartitions"));
    }

    private static void Dump(string name, PerformanceCounter[] counters, float? multiply = null)
    {
        Debug.WriteLine("----" + name);
        foreach (var counter in counters)
        {
            Debug.WriteLine(multiply.HasValue ? counter.NextValue() * multiply : counter.NextValue());
        }
    }

    private static PerformanceCounter[] Prepare(string categoryName, string counterName, string? instanceName = null)
    {
        var counters = Create(categoryName, counterName, instanceName);
        foreach (var counter in counters)
        {
            // 一度ダミーNextValue()の必要あり
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
