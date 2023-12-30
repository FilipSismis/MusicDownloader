using MusicDownloaderService.Model.Interface;
using System.Security;

namespace MusicDownloaderService
{
    public class ServiceConfig : IServiceAccountLogin
    {
        public string Username { get; set; }
        public SecureString Password { get; set; }
        public string MusicDirPath { get; set; }

        public ServiceConfig(IConfiguration config)
        {
            var userName = config.GetSection("ServiceAccountCredentials:Username").Value;
            if (string.IsNullOrEmpty(userName))
                throw new Exception("Username for service account is either empty or none in appsettings file");
            Username = userName;

            var password = config.GetSection("ServiceAccountCredentials:Password").Value;
            if (string.IsNullOrEmpty(password))
                throw new Exception("Password for service account is either empty or none in appsettings file");
            Password = new();

            var musicDirPath = config.GetSection("ServiceConfig:MusicDirPath").Value;
            if (string.IsNullOrEmpty(musicDirPath))
                throw new Exception("Music directory path for service account is either empty or none in appsettings file");
            MusicDirPath = musicDirPath;

            foreach (var c in password)
            {
                Password.AppendChar(c);
            }
        }
    }
}
