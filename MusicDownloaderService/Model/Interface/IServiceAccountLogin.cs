using System.Security;

namespace MusicDownloaderService.Model.Interface
{
    public interface IServiceAccountLogin
    {
        string Username { get; set; }
        SecureString Password { get; set; }
    }
}
