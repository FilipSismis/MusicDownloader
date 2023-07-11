using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicDownloader.Migrations
{
    /// <inheritdoc />
    public partial class columnUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastSong",
                table: "Playlists",
                newName: "LastSongId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastSongId",
                table: "Playlists",
                newName: "LastSong");
        }
    }
}
