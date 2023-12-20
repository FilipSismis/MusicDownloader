using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using MusicDownloaderService.DBAccess;
using MusicDownloaderService.Model.Interface;
using System.Diagnostics;
using VideoLibrary;

namespace MusicDownloaderService
{
    public class DownloadMusic : IDownloadMusic
    {
        private PlaylistRepository repo;
        private string musicDir;
        private string downloadDir;
        private Client<YouTubeVideo> ytClient;
        private VideoClient dlClient;
        private readonly IServiceAccountLogin serviceAccountLogin;
        private readonly ILogger<MusicDownloaderService> logger;
        private const string ytUrl = "https://www.youtube.com/watch?v=";
        public DownloadMusic(ILogger<MusicDownloaderService> logger, IConfiguration config, IServiceAccountLogin serviceAccountLogin)
        {
            this.logger = logger;
            this.serviceAccountLogin = serviceAccountLogin;

            ytClient = Client.For(new YouTube());
            dlClient = new();

            var connectionString = config.GetConnectionString("sqliteDbPath");
            if (string.IsNullOrEmpty(connectionString))
                throw new Exception("There is empty or none connection string in appsettings file");
            repo = new(connectionString);

            logger.LogInformation($"Service is in {Environment.CurrentDirectory}");
            var dir = Directory.GetParent(Environment.CurrentDirectory);
            if (dir != null)
                downloadDir = dir.FullName + @"\download";
            else
                throw new Exception($"Folder {Environment.CurrentDirectory} is a root folder, can't get a parent");

            if (!dir.GetDirectories().Select(i => i.Name).Contains("Music"))
            {
                logger.LogInformation("There is no Music dir, creating new one");
                Directory.CreateDirectory(dir.FullName + @"\Music");
            }

            musicDir = dir.FullName + @"\Music";
        }

        public async Task DownloadAsync()
        {
            var playlists = await repo.GetAllAsync();
            logger.LogInformation($"{playlists.Count} playlists have been retrieved from db");
            foreach (var playlist in playlists)
            {
                //retrieve playlist name if it's null in db
                if (string.IsNullOrEmpty(playlist.Name))
                {
                    logger.LogInformation("This playlist doesn't have a name in DB, retrieving from API...");
                    playlist.Name = await GetPlaylistName(playlist.Url);
                    repo.Update(playlist);
                }

                logger.LogInformation($"Retriving videos of {playlist.Name}");
                var videoIds = await GetVideosAsync(playlist.Url);

                //playlist is up to date so we just continue
                if (videoIds[0] == playlist.LastSongId)
                {
                    logger.LogInformation($"Playlist {playlist.Name} is up to date continuing to next one");
                    continue;
                }

                var dirList = Directory.GetDirectories(musicDir).Select(i => new DirectoryInfo(i).Name);
                if (!dirList.Contains(playlist.Name))
                {
                    logger.LogInformation($"Playlist {playlist.Name} doesn't have a folder creating");
                    Directory.CreateDirectory($@"{musicDir}\{playlist.Name}");
                }

                var playlistDir = musicDir + $@"\{playlist.Name}";

                foreach (var vidId in videoIds)
                {
                    if (vidId != playlist.LastSongId)
                    {
                        logger.LogInformation($"Downloading {vidId}...");
                        var vids = ytClient.GetAllVideosAsync(ytUrl + vidId).GetAwaiter().GetResult();
                        var vid = vids.OrderByDescending(i => i.AudioBitrate).First();
                        if (vid != null)
                        {
                            File.WriteAllBytes($@"{downloadDir}\{vid.FullName}", dlClient.GetBytes(vid));
                            logger.LogInformation("Video has been downloaded, starting the conversion");
                            ConvertVideo($@"{downloadDir}\{vid.FullName}", $@"{playlistDir}\{vid.FullName}.mp3");
                            logger.LogInformation("Video has been converted");
                        }
                    }
                    else
                        break;
                }
                logger.LogInformation($"Playlist {playlist.Name} is up to date, starting clean up");
                CleanUp();
                playlist.LastSongId = videoIds[0];
                repo.Update(playlist);
            }
            logger.LogInformation("All playlists are up to date");
        }

        public void ClientDispose()
        {
            ytClient.Dispose();
            dlClient.Dispose();
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
            ProcessStartInfo proccessInfo = new()
            {
                UserName = serviceAccountLogin.Username,
                Domain = "",
                Password = serviceAccountLogin.Password,
                UseShellExecute = false,
                FileName = "cmd.exe",
                Arguments = command,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            cmd.StartInfo = proccessInfo;
            cmd.Start();
            cmd.WaitForExit();
        }

        public void CleanUp()
        {
            var fileList = new DirectoryInfo(downloadDir).GetFiles();
            foreach (var file in fileList)
            {
                file.Delete();
            }
        }
    }
}
