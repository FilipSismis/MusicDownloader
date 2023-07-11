# MusicDownloader
Windows service written in c# to locally download and keep track of downloaded songs in your youtube playlists that you provide id for

##To run the service:
- build a db from a migration with ef core tools
- store your own youtube API key inside of environment variable called API_KEY
- for converting the music the service uses ffmpeg which can be downloaded from: [ffmpeg download](https://www.gyan.dev/ffmpeg/builds/)
- before running the service for now, you have to manually put IDs of your youtube playlists inside of the sqlite DB located in build folder

##Plans for future: 
- make a small GUI app to update the playlist IDs without having to temper with the DB itself
