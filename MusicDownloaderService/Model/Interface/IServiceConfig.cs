namespace MusicDownloaderService.Model.Interface
{
    public interface IServiceConfig
    {
        string MusicDirPath { get; set; }
        string DbPath { get; set; }
    }
}
