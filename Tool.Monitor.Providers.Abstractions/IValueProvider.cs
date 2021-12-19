namespace Tool.Monitor.Providers
{
    using System;

    public interface IValueProvider : IDisposable
    {
        IReadOnlyList<string> DataSources { get; }

        float?[] Collect(DateTime dateTime);
    }
}
