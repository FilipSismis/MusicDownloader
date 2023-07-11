using MusicDownloader;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<DownloadMusicService>();
    })
    .ConfigureAppConfiguration(conf =>
    {
        conf.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
    })
    .Build();

host.Run();
