namespace Tool.Monitor.Providers
{
    using System;

    /// <summary>
    ///
    /// </summary>
    public interface IValueProvider : IDisposable
    {
        /// <summary>
        ///
        /// </summary>
        string[] DataSources { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        float?[] Collect(DateTime dateTime);
    }
}
