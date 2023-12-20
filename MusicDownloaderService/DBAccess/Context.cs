using Microsoft.EntityFrameworkCore;
using MusicDownloaderService.Model.Entity;

namespace MusicDownloaderService.DBAccess
{
    public class Context : DbContext
    {
        public DbSet<Playlist> Playlists { get; set; }
        private string connectionString;
        public Context(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlite($"DataSource={connectionString}");
    }
}
