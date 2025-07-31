using System;
using Readarr.Core.Datastore;
using Readarr.Core.Qualities;
using Readarr.Core.Tv;
using System.Collections.Generic;

namespace Readarr.Core.History
{
    // Stub class for TV compatibility - to be removed
    public class EpisodeHistory : ModelBase
    {
        public int EpisodeId { get; set; }
        public int SeriesId { get; set; }
        public string SourceTitle { get; set; }
        public QualityModel Quality { get; set; }
        public DateTime Date { get; set; }
        public Episode Episode { get; set; }
        public Tv.Series Series { get; set; }
        public EpisodeHistoryEventType EventType { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public string DownloadId { get; set; }
    }

    public enum EpisodeHistoryEventType
    {
        Unknown = 0,
        Grabbed = 1,
        SeriesFolderImported = 2,
        DownloadFolderImported = 3,
        DownloadFailed = 4,
        EpisodeFileDeleted = 5,
        EpisodeFileRenamed = 6,
        DownloadIgnored = 7
    }
}