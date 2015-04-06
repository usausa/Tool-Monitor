namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DiskValueProvider : IValueProvider
    {
        private readonly DriveInfo[] driveInfos;

        public string[] DataSources { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public DiskValueProvider()
        {
            driveInfos = Enumerable.Range('A', 26)
                .Select(_ => new DriveInfo(((char)_).ToString(CultureInfo.InvariantCulture)))
                .Where(_ => _.IsReady && _.DriveType == DriveType.Fixed)
                .ToArray();
            DataSources = driveInfos.Select(_ => _.Name.Substring(0, 1)).ToArray();
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
            return driveInfos.Select(_ => (float?)(100 - (100f * _.TotalFreeSpace / _.TotalSize))).ToArray();
        }
    }
}
