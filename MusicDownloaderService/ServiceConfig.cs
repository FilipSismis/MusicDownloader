using MusicDownloaderService.Model.Interface;

namespace MusicDownloaderService
{
    public class ServiceConfig : IServiceConfig
    {
        public string MusicDirPath { get; set; }
        public string DbPath { get; set; }
        public string APIKey { get; set; }
        
        public ServiceConfig(IConfiguration config)
        {
            var musicDirPath = config.GetSection("ServiceConfig:MusicDirPath").Value;
            if (string.IsNullOrEmpty(musicDirPath))
                throw new Exception("Music directory path for service account is either empty or none in appsettings file");
            MusicDirPath = musicDirPath;

            var dbPath = config.GetSection("ServiceConfig:DbPath").Value;
            if (string.IsNullOrEmpty(dbPath))
                throw new Exception("Database path for service account is either empty or none in appsettings file");
            DbPath = dbPath;

            var apiKey = config.GetSection("ServiceConfig:APIKey").Value;
            if (string.IsNullOrEmpty(apiKey))
                throw new Exception("Api key is either empty or none in appsettings file");
            APIKey = apiKey;
        }
    }
}
