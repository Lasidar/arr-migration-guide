﻿using System.Collections.Generic;

namespace Readarr.Core.Download.Clients.UTorrent
{
    public class UTorrentTorrentCache
    {
        public string CacheID { get; set; }

        public List<UTorrentTorrent> Torrents { get; set; }
    }
}
