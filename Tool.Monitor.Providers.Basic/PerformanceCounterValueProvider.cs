namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public sealed class PerformanceCounterValueProvider : IValueProvider
    {
        private readonly PerformanceCounter[] counters;

        private readonly float multiply;

        public string[] DataSources { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="counters"></param>
        /// <param name="multiply"></param>
        /// <param name="dataSources"></param>
        public PerformanceCounterValueProvider(PerformanceCounter[] counters, float multiply, string[] dataSources)
        {
            this.counters = counters;
            this.multiply = multiply;
            DataSources = dataSources;
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            foreach (var counter in counters)
            {
                counter.Dispose();
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public float?[] Collect(DateTime dateTime)
        {
            return counters.Select(x => (float?)(x.NextValue() * multiply)).ToArray();
        }
    }
}
