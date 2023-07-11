using Microsoft.EntityFrameworkCore;

namespace MusicDownloader.DbAccess
{
    public class Context : DbContext
    {
        public DbSet<Playlist> Playlists { get; set; }
        public string DbPath { get; }

        public Context()
        {
            //change later can't use hardcoded path
#if DEBUG
            DbPath = Directory.GetParent(Environment.CurrentDirectory).FullName + @"\playlist.db";
#else
            //something for publish
#endif
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}
