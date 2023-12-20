namespace MusicDownloaderService.Model.Interface
{
    public interface IDownloadMusic
    {
        Task DownloadAsync();
        void ClientDispose();
    }
}
