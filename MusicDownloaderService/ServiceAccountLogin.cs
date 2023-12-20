using System.Security;
using MusicDownloaderService.Model.Interface;

namespace MusicDownloaderService
{
    public class ServiceAccountLogin : IServiceAccountLogin
    {
        public string Username { get; set; }
        public SecureString Password { get; set; }
        public ServiceAccountLogin(IConfiguration config)
        {
            var userName = config.GetSection("ServiceAccountCredentials:Username").Value;
            if (string.IsNullOrEmpty(userName))
                throw new Exception("Username for service account is either empty or none in appsettings file");
            Username = userName;

            var password = config.GetSection("ServiceAccountCredentials:Password").Value;
            if (string.IsNullOrEmpty(password))
                throw new Exception("Password for service account is either empty or none in appsettings file");

            Password = new();
            foreach (var character in password)
            {
                Password.AppendChar(character);
            }
        }
    }
}
