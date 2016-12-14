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

        public string[] DataSources { get; }

        /// <summary>
        ///
        /// </summary>
        public DiskValueProvider()
        {
            driveInfos = Enumerable.Range('A', 26)
                .Select(x => new DriveInfo(((char)x).ToString(CultureInfo.InvariantCulture)))
                .Where(x => x.IsReady && x.DriveType == DriveType.Fixed)
                .ToArray();
            DataSources = driveInfos.Select(x => x.Name.Substring(0, 1)).ToArray();
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
            return driveInfos.Select(x => (float?)(100 - (100f * x.TotalFreeSpace / x.TotalSize))).ToArray();
        }
    }
}
