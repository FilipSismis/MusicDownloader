using Microsoft.EntityFrameworkCore;

namespace MusicDownloader.DbAccess
{
    public class PlaylistRepository
    {
        private Context context;
        public PlaylistRepository()
        {
            context = new Context();
        }

        public async Task<List<Playlist>> GetAllAsync() => await context.Playlists.ToListAsync();

        public async void Update(Playlist playlist)
        {
            var oldPlaylist = await context.Playlists.FirstOrDefaultAsync(i => i.Id == playlist.Id);
            if(oldPlaylist != null)
            {
                oldPlaylist.Name = playlist.Name;
                oldPlaylist.Url = playlist.Url;
                oldPlaylist.LastSongId = playlist.LastSongId;
            }
            await context.SaveChangesAsync();
        }
    }
}
