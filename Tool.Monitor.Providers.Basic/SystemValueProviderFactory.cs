namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Linq;

    public class SystemValueProviderFactory : IValueProviderFactory
    {
        public void Initialize(IDictionary<string, string[]> parameters)
        {
        }

        public IValueProvider Create(IDictionary<string, string[]> parameters)
        {
            return parameters["Type"].First() switch
            {
                "Disk" => new DiskValueProvider(),
                "Process" => new ProcessValueProvider(),
                _ => throw new ArgumentException("Type unknown.", nameof(parameters))
            };
        }
    }
}
