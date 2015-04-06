namespace Tool.Monitor.Providers.Hardware
{
    using System.Linq;

    using OpenHardwareMonitor.Hardware;

    /// <summary>
    /// 
    /// </summary>
    public class HardwareValueProviderFactory : IValueProviderFactory
    {
        private HardwareMonitor monitor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void Initialize(ILookup<string, string> parameters)
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

            monitor = new HardwareMonitor(computer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IValueProvider Create(ILookup<string, string> parameters)
        {
            return new HardwareValueProvider(monitor, parameters["Key"].ToArray());
        }
    }
}
