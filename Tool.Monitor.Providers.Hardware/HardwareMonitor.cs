namespace Tool.Monitor.Providers.Hardware
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using OpenHardwareMonitor.Hardware;

    /// <summary>
    ///
    /// </summary>
    public class HardwareMonitor
    {
        private readonly Computer computer;

        private readonly Dictionary<string, float?> infomations = new Dictionary<string, float?>();

        private readonly UpdateVisitor updateVisitor = new UpdateVisitor();

        private readonly SensorVisitor sensorVisitor;

        private DateTime lastUpdated = DateTime.MinValue;

        /// <summary>
        ///
        /// </summary>
        /// <param name="computer"></param>
        public HardwareMonitor(Computer computer)
        {
            this.computer = computer;
            sensorVisitor = new SensorVisitor(sensor =>
            {
                infomations[sensor.Identifier.ToString()] = sensor.Value;
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public float?[] GetValues(DateTime dateTime, IEnumerable<string> keys)
        {
            if (lastUpdated != dateTime)
            {
                infomations.Clear();

                updateVisitor.VisitComputer(computer);
                sensorVisitor.VisitComputer(computer);

                lastUpdated = dateTime;
            }

            return keys.Select(_ =>
            {
                float? value;
                return infomations.TryGetValue(_, out value) ? value : null;
            }).ToArray();
        }
    }
}
