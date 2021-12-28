namespace WorkPeriod;

using System.Diagnostics;
using System.Threading;

public static class Program
{
    public static async Task Main()
    {
        var now = DateTime.Now;
        Debug.WriteLine($"**** Start [{now:HH:mm:ss.fff}]");

        var timer = new IntervalTimer(TimeSpan.FromSeconds(5));
        for (var i = 0; i < 10; i++)
        {
            await timer.WaitAsync();
            Debug.WriteLine($"**** Action [{DateTime.Now:HH:mm:ss.fff}]");
        }
    }
}

public class IntervalTimer
{
    private readonly TimeSpan span;

    private DateTime previous;

    public IntervalTimer(TimeSpan span)
    {
        this.span = span;
        var now = DateTime.Now;
        var second = span < TimeSpan.FromMinutes(1) ? (now.Second / span.Seconds) * span.Seconds : 0;
        previous = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, second);
    }

    public async ValueTask WaitAsync(CancellationToken cancellation = default)
    {
        var next = previous + span;
        previous = next;
        var wait = (int)(next - DateTime.Now).TotalMilliseconds;
        if (wait < 0)
        {
            return;
        }

        await Task.Delay(wait, cancellation);
    }
}
