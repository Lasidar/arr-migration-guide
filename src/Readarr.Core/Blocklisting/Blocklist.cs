using System;
using System.Collections.Generic;
using Readarr.Core.Datastore;
using Readarr.Core.Indexers;
using Readarr.Core.Languages;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Core.Tv;

namespace Readarr.Core.Blocklisting
{
    public class Blocklist : ModelBase
    {
        public int SeriesId { get; set; }
        public Series Series { get; set; }
        public List<int> EpisodeIds { get; set; }
        public string SourceTitle { get; set; }
        public QualityModel Quality { get; set; }
        public DateTime Date { get; set; }
        public DateTime? PublishedDate { get; set; }
        public long? Size { get; set; }
        public DownloadProtocol Protocol { get; set; }
        public string Indexer { get; set; }
        public IndexerFlags IndexerFlags { get; set; }
        public ReleaseType ReleaseType { get; set; }
        public string Message { get; set; }
        public string TorrentInfoHash { get; set; }
        public List<Language> Languages { get; set; }
    }
}
