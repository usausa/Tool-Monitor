namespace Tool.Monitor
{
    using System;
    using System.Linq;

    using Quartz;

    /// <summary>
    /// 
    /// </summary>
    public class MonitorJob : IJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            var dataMap = context.MergedJobDataMap;
            var rrdTool = (RRDTool)dataMap["RRDTool"];
            var collectors = (CollectorInformation[])dataMap["Collectors"];

            var now = DateTime.Now;
            var dayStart = now.AddDays(-1).ToUnixTime();
            var weekStart = now.AddDays(-7).ToUnixTime();
            var monthStart = now.AddDays(-30).ToUnixTime();
            var yearStart = now.AddDays(-365).ToUnixTime();
            var dateTime = now.ToUnixTime();

            foreach (var set in collectors.Select(_ => new { Collector = _, Values = _.ValueProvider.Collect(now) }).ToList())
            {
                var id = set.Collector.Id;
                var go = set.Collector.GraphOption;
                var ds = set.Collector.ValueProvider.DataSources;

                if (set.Values != null)
                {
                    rrdTool.Update(id, dateTime, set.Values);
                }

                rrdTool.Graph(id, "day", dayStart, dateTime, go.Draw, go.Title, go.Option, go.Colors, ds, go.Labels.Length > 0 ? go.Labels : ds);
                rrdTool.Graph(id, "week", weekStart, dateTime, go.Draw, go.Title, go.Option, go.Colors, ds, go.Labels.Length > 0 ? go.Labels : ds);
                rrdTool.Graph(id, "month", monthStart, dateTime, go.Draw, go.Title, go.Option, go.Colors, ds, go.Labels.Length > 0 ? go.Labels : ds);
                rrdTool.Graph(id, "year", yearStart, dateTime, go.Draw, go.Title, go.Option, go.Colors, ds, go.Labels.Length > 0 ? go.Labels : ds);
            }
        }
    }
}
