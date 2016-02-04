namespace Tool.Monitor
{
    using System;
    using System.Globalization;
    using System.IO;

    using Quartz;
    using Quartz.Impl;

    using RazorEngine.Configuration;
    using RazorEngine.Templating;

    /// <summary>
    ///
    /// </summary>
    public static class Program
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Program));

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // ログ設定
            log4net.Config.XmlConfigurator.Configure();

            // コンテキストロード
            var loader = new XmlContextLoader();
            var config = loader.Load(args.Length > 0 ? args[0] : "Settings.config");

            // ファイル初期化
            PrepareRRDTool(config);

            // テンプレート生成
            CreateTemplate(config);

            // スケジューラー
            var factory = new StdSchedulerFactory();
            var scheduler = factory.GetScheduler();
            var dataMap = new JobDataMap { { "RRDTool", config.RRDTool }, { "Collectors", config.Collectors } };

            scheduler.ScheduleJob(
                JobBuilder.Create<MonitorJob>().WithIdentity("Monitor").SetJobData(dataMap).Build(),
                TriggerBuilder.Create().StartNow().WithCronSchedule("0 0/5 * * * ?").Build());

            Log.Info("Monitor start.");

            scheduler.Start();

            Console.ReadKey();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        private static void PrepareRRDTool(Config config)
        {
            Directory.CreateDirectory(config.RRDTool.DataDir);
            Directory.CreateDirectory(config.RRDTool.OutputDir);
            foreach (var collector in config.Collectors)
            {
                config.RRDTool.Prepare(collector.Id, collector.GraphOption.SourceType, collector.ValueProvider.DataSources);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="config"></param>
        private static void CreateTemplate(Config config)
        {
            var listTemplate = File.ReadAllText(config.Template.ListTemplate, System.Text.Encoding.UTF8);
            var detailTemplate = File.ReadAllText(config.Template.DetailTemplate, System.Text.Encoding.UTF8);

            var serviceConfig = new TemplateServiceConfiguration { CachingProvider = new DefaultCachingProvider(t => { }) };
            using (var service = RazorEngineService.Create(serviceConfig))
            {
                File.WriteAllText(
                    Path.Combine(config.Template.OutputDir, config.Template.ListFileName),
                    service.RunCompile(listTemplate, "list", typeof(CollectorInformation[]), config.Collectors),
                    System.Text.Encoding.UTF8);
                foreach (var collector in config.Collectors)
                {
                    File.WriteAllText(
                        Path.Combine(
                            config.Template.OutputDir,
                            String.Format(CultureInfo.InvariantCulture, config.Template.DetailFileName, collector.Id)),
                        service.RunCompile(detailTemplate, "detail", typeof(CollectorInformation), collector),
                        System.Text.Encoding.UTF8);
                }
            }
        }
    }
}
