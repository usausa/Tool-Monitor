namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Versioning;

    [SupportedOSPlatform("windows")]
    public class PerformanceCounterValueProviderFactory : IValueProviderFactory
    {
        public void Initialize(IDictionary<string, string[]> parameters)
        {
        }

        public IValueProvider Create(IDictionary<string, string[]> parameters)
        {
            var category = parameters["Category"].First();
            var counter = parameters["Counter"].First();
            var instance = parameters.TryGetValue("Instance", out var instanceValues) ? instanceValues.First() : null;
            var multiply = parameters.TryGetValue("Multiply", out var multiplyValues) ? Single.Parse(multiplyValues.First(), CultureInfo.InvariantCulture) : 1f;

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
