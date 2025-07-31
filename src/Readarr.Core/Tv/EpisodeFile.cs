using System;
using Readarr.Core.Datastore;
using Readarr.Core.MediaFiles;
using Readarr.Core.Qualities;

namespace Readarr.Core.Tv
{
    // Stub class for TV compatibility - to be removed
    public class EpisodeFile : ModelBase
    {
        public int SeriesId { get; set; }
        public int SeasonNumber { get; set; }
        public string RelativePath { get; set; }
        public string Path { get; set; }
        public long Size { get; set; }
        public DateTime DateAdded { get; set; }
        public string SceneName { get; set; }
        public string ReleaseGroup { get; set; }
        public QualityModel Quality { get; set; }
        public MediaInfoModel MediaInfo { get; set; }
        public string OriginalFilePath { get; set; }
        public bool QualityCutoffNotMet { get; set; }
        public string ReleaseHash { get; set; }
        public string IndexerFlags { get; set; }
    }
}