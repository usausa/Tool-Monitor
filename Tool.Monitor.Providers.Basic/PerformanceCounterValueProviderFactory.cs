namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public class PerformanceCounterValueProviderFactory : IValueProviderFactory
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
            var category = parameters["Category"].First();
            var counter = parameters["Counter"].First();
            var instance = parameters.Contains("Instance") ? parameters["Instance"].First() : null;
            var multiply = parameters.Contains("Multiply") ? Single.Parse(parameters["Multiply"].First(), CultureInfo.InvariantCulture) : 1f;

            if (instance != null)
            {
                return new PerformanceCounterValueProvider(new[] { new PerformanceCounter(category, counter, instance) }, multiply, new[] { instance });
            }

            var pcc = new PerformanceCounterCategory(category);
            if (pcc.CategoryType == PerformanceCounterCategoryType.SingleInstance)
            {
                return new PerformanceCounterValueProvider(new[] { new PerformanceCounter(category, counter) }, multiply, new[] { "x" });
            }

            var instanceNames = pcc.GetInstanceNames().OrderBy(x => x).ToArray();
            return new PerformanceCounterValueProvider(
                instanceNames.Select(x => new PerformanceCounter(category, counter, x)).ToArray(), multiply, instanceNames);
        }
    }
}
