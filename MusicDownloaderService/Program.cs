using MusicDownloaderService;
using MusicDownloaderService.Model.Interface;
using Serilog;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json").Build();

var logPath = config["Logging:Logpath"];

if (string.IsNullOrEmpty(logPath))
    throw new Exception("There is empty or none log path for log file in appsettings");

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("microsoft", Serilog.Events.LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.File(logPath)
    .WriteTo.Console()
    .CreateLogger();

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureServices(services =>
    {
        services.AddSingleton<IServiceConfig, ServiceConfig>();
        services.AddSingleton<IDownloadMusic, DownloadMusic>();
        services.AddHostedService<MusicDownloaderService.MusicDownloaderService>();
    }).UseSerilog()
    .Build();



host.Run();
