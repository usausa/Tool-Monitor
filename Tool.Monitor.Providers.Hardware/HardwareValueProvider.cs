namespace Tool.Monitor.Providers.Hardware
{
    using System;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public sealed class HardwareValueProvider : IValueProvider
    {
        private readonly HardwareMonitor monitor;

        private readonly string[] keys;

        public string[] DataSources { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="monitor"></param>
        /// <param name="keys"></param>
        public HardwareValueProvider(HardwareMonitor monitor, string[] keys)
        {
            this.monitor = monitor;
            this.keys = keys;
            DataSources = Enumerable.Range(0, keys.Length).Select(_ => _.ToString(CultureInfo.InvariantCulture)).ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public float?[] Collect(DateTime dateTime)
        {
            return monitor.GetValues(dateTime, keys);
        }
    }
}
