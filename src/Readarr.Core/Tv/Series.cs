using System;
using Readarr.Core.Datastore;

namespace Readarr.Core.Tv
{
    // Stub class for TV compatibility - to be removed
    public class Series : ModelBase
    {
        public int TvdbId { get; set; }
        public int TvRageId { get; set; }
        public int TvMazeId { get; set; }
        public string ImdbId { get; set; }
        public string Title { get; set; }
        public string CleanTitle { get; set; }
        public string SortTitle { get; set; }
        public SeriesStatusType Status { get; set; }
        public string Overview { get; set; }
        public string AirTime { get; set; }
        public bool Monitored { get; set; }
        public int QualityProfileId { get; set; }
        public bool SeasonFolder { get; set; }
        public DateTime? LastInfoSync { get; set; }
        public int Runtime { get; set; }
        public string Images { get; set; }
        public SeriesTypes SeriesType { get; set; }
        public string Network { get; set; }
        public bool UseSceneNumbering { get; set; }
        public string TitleSlug { get; set; }
        public string Path { get; set; }
        public int Year { get; set; }
        public string Ratings { get; set; }
        public string Genres { get; set; }
        public string Actors { get; set; }
        public string Certification { get; set; }
        public string RootFolderPath { get; set; }
        public DateTime Added { get; set; }
        public DateTime? FirstAired { get; set; }
        public DateTime? LastAired { get; set; }
        public int LanguageProfileId { get; set; }
        public int? TvdbSeasonId { get; set; }
        public string Tags { get; set; }
    }

    public enum SeriesStatusType
    {
        Continuing = 0,
        Ended = 1,
        Upcoming = 2,
        Deleted = 3
    }
}