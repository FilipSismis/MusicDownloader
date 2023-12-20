namespace MusicDownloaderService.Model.Entity
{
    public class Playlist
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public required string Url { get; set; }
        public string? LastSongId { get; set; }
    }
}
