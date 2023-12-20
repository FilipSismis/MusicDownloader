using MusicDownloaderService.Model.Interface;

namespace MusicDownloaderService
{
    public class MusicDownloaderService : BackgroundService
    {
        private readonly ILogger<MusicDownloaderService> logger;
        private readonly IDownloadMusic downloadMusic;

        public MusicDownloaderService(ILogger<MusicDownloaderService> logger, IDownloadMusic downloadMusic)
        {
            this.logger = logger;
            this.downloadMusic = downloadMusic;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting service");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping service");
            downloadMusic.ClientDispose();
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Executing Service");
            try
            {
                await downloadMusic.DownloadAsync();
                logger.LogInformation("Service has finished execution");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
