namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public sealed class ProcessValueProvider : IValueProvider
    {
        public IReadOnlyList<string> DataSources { get; }

        public ProcessValueProvider()
        {
            DataSources = new[] { "processes", "threads" };
        }

        public void Dispose()
        {
        }

        public float?[] Collect(DateTime dateTime)
        {
            var processes = Process.GetProcesses().ToArray();
            return new float?[] { processes.Length, processes.Sum(x => x.Threads.Count) };
        }
    }
}
