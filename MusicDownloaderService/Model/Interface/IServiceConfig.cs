using System.Security;

namespace MusicDownloaderService.Model.Interface
{
    public interface IServiceConfig
    {
        string Username { get; set; }
        SecureString Password { get; set; }
        string MusicDirPath { get; set; }
        string DbPath { get; set; }
    }
}
