namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public class SystemValueProviderFactory : IValueProviderFactory
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters"></param>
        public void Initialize(ILookup<string, string> parameters)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IValueProvider Create(ILookup<string, string> parameters)
        {
            switch (parameters["Type"].First())
            {
                case "Disk":
                    return new DiskValueProvider();
                case "Process":
                    return new ProcessValueProvider();
            }

            throw new ArgumentException("Type unknown.", nameof(parameters));
        }
    }
}
