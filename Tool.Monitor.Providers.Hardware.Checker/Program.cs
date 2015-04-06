namespace Tool.Monitor.Providers.Hardware.Checker
{
    using System;

    using OpenHardwareMonitor.Hardware;

    /// <summary>
    /// 
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// 
        /// </summary>
        public static void Main()
        {
            var computer = new Computer
            {
                MainboardEnabled = true,
                CPUEnabled = true,
                RAMEnabled = true,
                GPUEnabled = true,
                FanControllerEnabled = true,
                HDDEnabled = true
            };
            computer.Open();

            var updateVisitor = new UpdateVisitor();
            var sensorVisitor = new SensorVisitor(sensor =>
            {
                Console.WriteLine(sensor.Identifier);
                Console.WriteLine(sensor.Hardware.Name);
                Console.WriteLine(sensor.Name);
                Console.WriteLine(sensor.SensorType);
                Console.WriteLine(sensor.Value);
                Console.WriteLine();
            });

            updateVisitor.VisitComputer(computer);
            sensorVisitor.VisitComputer(computer);
        }
    }
}
