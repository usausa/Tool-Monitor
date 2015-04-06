namespace Tool.Monitor.Providers.Hardware
{
    using OpenHardwareMonitor.Hardware;

    /// <summary>
    /// 
    /// </summary>
    public class UpdateVisitor : IVisitor
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="computer"></param>
        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hardware"></param>
        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();
            foreach (var subHardware in hardware.SubHardware)
            {
                subHardware.Accept(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sensor"></param>
        public void VisitSensor(ISensor sensor)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void VisitParameter(IParameter parameter)
        {
        }
    }
}
