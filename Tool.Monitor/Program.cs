using Serilog;

using Tool.Monitor;

#pragma warning disable CA1812

var host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging((_, logging) =>
    {
        logging.ClearProviders();
    })
    .UseSerilog((hostingContext, loggerConfiguration) =>
    {
        loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
    })
    .ConfigureServices(services =>
    {
        // TODO config
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
