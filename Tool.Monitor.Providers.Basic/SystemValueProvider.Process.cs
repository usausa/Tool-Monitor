﻿namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ProcessValueProvider : IValueProvider
    {
        public string[] DataSources { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ProcessValueProvider()
        {
            DataSources = new[] { "processes", "threads" };
        }
        
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public float?[] Collect(DateTime dateTime)
        {
            var processes = Process.GetProcesses().ToArray();
            return new float?[] { processes.Length, processes.Sum(_ => _.Threads.Count) };
        }
    }
}
