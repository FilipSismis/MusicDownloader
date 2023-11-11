﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicDownloader
{
    public class Playlist
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public required string Url { get; set; }
        public string? LastSongId { get; set; }
    }
}