# MusicDownloader
Windows service written in c# to locally download and keep track of downloaded songs in your youtube playlists that you provide id for

## To run the service:
- build a db from a migration with ef core tools
- for converting the music the service uses ffmpeg which can be downloaded from: [ffmpeg download](https://www.gyan.dev/ffmpeg/builds/)

## Config file structure i.e. appsettigs.json
```
{
  "Logging": {
    "LogPath": "path\\to\\log_file.txt",
    "LogLevel": {
      "Default": "Information",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "ServiceConfig": {
    "Username" : "usernameForFfmpegProccess",
    "Password" : "passwordForFfmpegProccess",
    "APIKey" : "api_key_to_youtube_api"
    "DbPath" : "path\\to\\sqlite.db
    "MusicDirpath" : "music\\directory\\path\\"
  }
}
