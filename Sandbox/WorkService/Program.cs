using System.Reflection;

using Serilog;

var builder = Host.CreateDefaultBuilder(args);

builder.UseWindowsService();
Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);

builder
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
    })
    .UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    });

builder.ConfigureServices(services =>
{
    services.AddHostedService<Worker>();
});

var host = builder.Build();

await host.RunAsync();

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> logger;

    public Worker(ILogger<Worker> logger)
    {
        this.logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var assembly = Assembly.LoadFrom("WorkService.Plugin.dll");
        logger.LogInformation("Assembly name=[{Name}]", assembly.FullName);

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(5000, stoppingToken);
        }
    }
}
