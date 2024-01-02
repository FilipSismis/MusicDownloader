using System.Security;

namespace MusicDownloaderService.Model.Interface
{
    public interface IServiceConfig
    {
        string MusicDirPath { get; set; }
        string DbPath { get; set; }
        string APIKey { get; set; }
        string Username { get; set; }
        SecureString Password { get; set; }
    }
}
