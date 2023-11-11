using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using log4net;
using MusicDownloader.DbAccess;
using System.Diagnostics;
using VideoLibrary;

namespace MusicDownloader
{
    public sealed class DownloadMusicService : BackgroundService
    {
        private PlaylistRepository? repo;
        private string? musicDir;
        private string? downloadDir;
        private Client<YouTubeVideo>? ytClient;
        private VideoClient? dlClient;
        private const string ytUrl = "https://www.youtube.com/watch?v=";
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            ytClient = Client.For(new YouTube());
            dlClient = new();
            repo = new();
            
            var dir = Directory.GetParent(Environment.CurrentDirectory);
            if (dir != null)
                downloadDir = dir.FullName + @"\download";
            else
            {
                log.Error($"Folder {Environment.CurrentDirectory} is a root folder, can't get a parent");
                return Task.CompletedTask;
            }

            var parentDir = dir.Parent;
            if (parentDir != null)
            {
                if (!parentDir.GetDirectories().Select(i => i.Name).Contains("Music"))
                    Directory.CreateDirectory(parentDir.FullName + @"\Music");
                musicDir = dir.Parent + @"\Music";
            }
            else
            {
                log.Error($"Folder {dir} is a root folder, can't get a parent");
                return Task.CompletedTask;
            }
            log.Info("Starting the Service with initialized directories");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            ytClient?.Dispose();
            dlClient?.Dispose();
            log.Info("Stopping the service");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var playlists = await repo.GetAllAsync();
            foreach (var playlist in playlists)
            {
                //retrieve playlist name if it's null in db
                if (string.IsNullOrEmpty(playlist.Name))
                {
                    playlist.Name = await GetPlaylistName(playlist.Url);
                    repo.Update(playlist);
                }

                var videoIds = await GetVideosAsync(playlist.Url);

                //playlist is up to date so we just continue
                if (videoIds[0] == playlist.LastSongId)
                    continue;

                var dirList = Directory.GetDirectories(musicDir).Select(i => new DirectoryInfo(i).Name);
                if (!dirList.Contains(playlist.Name))
                    Directory.CreateDirectory($@"{musicDir}\{playlist.Name}");

                var playlistDir = musicDir + $@"\{playlist.Name}";

                foreach (var vidId in videoIds)
                {
                    if (vidId != playlist.LastSongId)
                    {
                        var vids = ytClient.GetAllVideosAsync(ytUrl + vidId).GetAwaiter().GetResult();
                        var vid = vids.OrderByDescending(i => i.AudioBitrate).First();
                        if (vid != null)
                        {
                            File.WriteAllBytes($@"{downloadDir}\{vid.FullName}", dlClient.GetBytes(vid));
                            ConvertVideo($@"{downloadDir}\{vid.FullName}", $@"{playlistDir}\{vid.FullName}.mp3");
                        }
                    }
                    else
                        break;
                }

                CleanUp();
                playlist.LastSongId = videoIds[0];
                repo.Update(playlist);
            }
        }

        private async Task<List<string>> GetVideosAsync(string playlistId)
        {
            List<string> result = new List<string>();
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Environment.GetEnvironmentVariable("API_KEY"),
                ApplicationName = "AutoPlaylistUpdate"
            });

            string nextPageToken = "";
            while (nextPageToken != null)
            {
                var request = youtubeService.PlaylistItems.List("contentDetails");
                request.PlaylistId = playlistId;
                request.MaxResults = 50;
                request.PageToken = nextPageToken;

                var response = await request.ExecuteAsync();

                foreach (var item in response.Items)
                {
                    result.Add(item.ContentDetails.VideoId);
                }

                nextPageToken = response.NextPageToken;
            }

            return result;
        }

        private async Task<string> GetPlaylistName(string playlistId)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = Environment.GetEnvironmentVariable("API_KEY"),
                ApplicationName = "AutoPlaylistUpdate"
            });

            var request = youtubeService.Playlists.List("snippet");
            request.Id = playlistId;

            var response = await request.ExecuteAsync();

            return response.Items.First().Snippet.Title;
        }

        private void ConvertVideo(string oldFile, string newFile)
        {
            string command = $"/C ffmpeg -i \"{oldFile}\" -ar 44100 -ac 2 -b:a 128k \"{newFile}\"";
            Process cmd = new();
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.Arguments = command;
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.Start();
            cmd.WaitForExit();
        }

        private void CleanUp()
        {
            var fileList = new DirectoryInfo(downloadDir).GetFiles();
            foreach (var file in fileList)
            {
                file.Delete();
            }
        }
    }
}
