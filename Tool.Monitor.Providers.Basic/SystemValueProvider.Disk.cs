namespace Tool.Monitor.Providers.Basic
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public sealed class DiskValueProvider : IValueProvider
    {
        private readonly DriveInfo[] driveInfos;

        public IReadOnlyList<string> DataSources { get; }

        public DiskValueProvider()
        {
            driveInfos = Enumerable.Range('A', 26)
                .Select(x => new DriveInfo(((char)x).ToString(CultureInfo.InvariantCulture)))
                .Where(x => x.IsReady && x.DriveType == DriveType.Fixed)
                .ToArray();
            DataSources = driveInfos.Select(x => x.Name[..1]).ToArray();
        }

        public void Dispose()
        {
        }

        public float?[] Collect(DateTime dateTime)
        {
            return driveInfos.Select(x => (float?)(100 - (100f * x.TotalFreeSpace / x.TotalSize))).ToArray();
        }
    }
}
