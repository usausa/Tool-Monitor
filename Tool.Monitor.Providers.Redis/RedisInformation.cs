namespace Tool.Monitor.Providers.Redis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    ///
    /// </summary>
    public class RedisInformation
    {
        private readonly string client;

        private readonly string option;

        private Dictionary<string, string> infomations;

        private DateTime lastUpdated = DateTime.MinValue;

        /// <summary>
        ///
        /// </summary>
        /// <param name="client"></param>
        /// <param name="option"></param>
        public RedisInformation(string client, string option)
        {
            this.client = client;
            this.option = option;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="keys"></param>
        /// <returns></returns>
        public float?[] GetValues(DateTime dateTime, IEnumerable<string> keys)
        {
            if (lastUpdated != dateTime)
            {
                UpdateInformation();

                lastUpdated = dateTime;
            }

            return keys.Select(x =>
            {
                string value;
                return infomations.TryGetValue(x, out value) ? (float?)Single.Parse(value, CultureInfo.InvariantCulture) : null;
            }).ToArray();
        }

        /// <summary>
        ///
        /// </summary>
        private void UpdateInformation()
        {
            using (var proc = new Process())
            {
                var psi = new ProcessStartInfo
                {
                    FileName = client,
                    Arguments = option,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                proc.StartInfo = psi;
                proc.Start();
                if (!proc.WaitForExit(10000))
                {
                    proc.Kill();
                }

                var output = proc.StandardOutput.ReadToEnd();
                infomations = output.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(':'))
                    .Where(x => x.Length > 1)
                    .ToDictionary(x => x[0], x => x[1].Replace("\r", string.Empty));
            }
        }
    }
}
