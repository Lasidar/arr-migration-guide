using System;
using Readarr.Core.Datastore;
using Readarr.Core.Tv;

namespace Readarr.Core.Tv
{
    // Stub class for TV compatibility - to be removed
    public class Episode : ModelBase
    {
        public int SeriesId { get; set; }
        public int EpisodeFileId { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeNumber { get; set; }
        public string Title { get; set; }
        public string AirDate { get; set; }
        public DateTime? AirDateUtc { get; set; }
        public int Runtime { get; set; }
        public bool Monitored { get; set; }
        public int? AbsoluteEpisodeNumber { get; set; }
        public int? SceneAbsoluteEpisodeNumber { get; set; }
        public int? SceneSeasonNumber { get; set; }
        public int? SceneEpisodeNumber { get; set; }
        public bool UnverifiedSceneNumbering { get; set; }
        public bool HasFile { get; set; }
    }
}