namespace Tool.Monitor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    public class RRDTool
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(RRDTool));

        public string ExePath { get; set; }

        public string DataDir { get; set; }

        public string OutputDir { get; set; }

        public string[] Colors { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sourceType"></param>
        /// <param name="dataSources"></param>
        public void Prepare(string id, string sourceType, IEnumerable<string> dataSources)
        {
            var filename = Path.Combine(DataDir, String.Format(CultureInfo.InvariantCulture, "{0}.rrd", id));
            if (File.Exists(filename))
            {
                return;
            }

            var sb = new StringBuilder();
            sb.Append("create \"");
            sb.Append(filename);
            sb.Append("\"");
            foreach (var dataSource in dataSources)
            {
                sb.Append(" DS:");
                sb.Append(dataSource);
                sb.Append(":");
                sb.Append(sourceType);
                sb.Append(":600:U:U");
            }
            sb.Append(" RRA:AVERAGE:0.5:1:735840");

            Run(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="time"></param>
        /// <param name="values"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Ignore")]
        public void Update(string id, int time, IEnumerable<float?> values)
        {
            var sb = new StringBuilder();
            sb.Append("update \"");
            sb.Append(Path.Combine(DataDir, String.Format(CultureInfo.InvariantCulture, "{0}.rrd", id)));
            sb.Append("\" ");
            sb.Append(time);
            sb.Append(":");
            foreach (var value in values)
            {
                if (value.HasValue)
                {
                    sb.Append(value);
                }
                else
                {
                    sb.Append("U");
                }
                sb.Append(":");
            }
            sb.Length = sb.Length - 1;

            Run(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="suffix"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="draw"></param>
        /// <param name="title"></param>
        /// <param name="option"></param>
        /// <param name="colors"></param>
        /// <param name="dataSources"></param>
        /// <param name="labels"></param>
        public void Graph(string id, string suffix, int start, int end, string draw, string title, string option, string[] colors, string[] dataSources, string[] labels)
        {
            var sb = new StringBuilder();
            sb.Append("graph \"");
            sb.Append(Path.Combine(OutputDir, String.Format(CultureInfo.InvariantCulture, "{0}-{1}.png", id, suffix)));
            sb.Append("\" --start ");
            sb.Append(start);
            sb.Append(" --end ");
            sb.Append(end);
            sb.Append(" --title ");
            sb.Append("\"");
            sb.Append(title);
            sb.Append("\"");
            if (!String.IsNullOrEmpty(option))
            {
                sb.Append(" ");
                sb.Append(option);
            }
            for (var i = 0; i < dataSources.Length; i++)
            {
                sb.Append(" DEF:value");
                sb.Append(i);
                sb.Append("=\"");
                sb.Append(Path.Combine(DataDir, String.Format(CultureInfo.InvariantCulture, "{0}.rrd", id)));
                sb.Append("\":");
                sb.Append(dataSources[i]);
                sb.Append(":AVERAGE");
            }
            sb.Append(" COMMENT:\"                    Cur          Avg          Min          Max\\n\"");
            for (var i = 0; i < labels.Length; i++) 
            {
                sb.Append(" ");
                sb.Append(draw);
                sb.Append(":value");
                sb.Append(i);

                sb.Append((colors != null) && (colors.Length > 0) ? colors[i % colors.Length] : Colors[i % Colors.Length]);
                sb.Append(":\"");
                sb.Append(labels[i].PadRight(10, ' ').Substring(0, 10));
                sb.Append("\"");

                sb.Append(" GPRINT:value");
                sb.Append(i);
                sb.Append(":LAST:\"%8.2lf\"");

                sb.Append(" GPRINT:value");
                sb.Append(i);
                sb.Append(":AVERAGE:\"%8.2lf\"");

                sb.Append(" GPRINT:value");
                sb.Append(i);
                sb.Append(":MIN:\"%8.2lf\"");

                sb.Append(" GPRINT:value");
                sb.Append(i);
                sb.Append(":MAX:\"%8.2lf\"");
            }

            Run(sb.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arguments"></param>
        private void Run(string arguments)
        {
            using (var proc = new Process())
            {
                var psi = new ProcessStartInfo
                {
                    FileName = ExePath,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardError = true
                };
                proc.StartInfo = psi;
                proc.Start();
                if (!proc.WaitForExit(10000))
                {
                    proc.Kill();
                }

                var error = proc.StandardError.ReadToEnd();
                if (!String.IsNullOrEmpty(error))
                {
                    Log.Warn(error);
                }
            }
        }
    }
}
