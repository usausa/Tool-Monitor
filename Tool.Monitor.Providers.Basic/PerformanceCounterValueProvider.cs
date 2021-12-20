namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Versioning;

    [SupportedOSPlatform("windows")]
    public sealed class PerformanceCounterValueProvider : IValueProvider
    {
        private readonly PerformanceCounter[] counters;

        private readonly float multiply;

        public IReadOnlyList<string> DataSources { get; }

        public PerformanceCounterValueProvider(PerformanceCounter[] counters, float multiply, string[] dataSources)
        {
            this.counters = counters;
            this.multiply = multiply;
            DataSources = dataSources;
        }

        public void Dispose()
        {
            foreach (var counter in counters)
            {
                counter.Dispose();
            }
        }

        public float?[] Collect(DateTime dateTime)
        {
            return counters.Select(x => (float?)(x.NextValue() * multiply)).ToArray();
        }
    }
}
