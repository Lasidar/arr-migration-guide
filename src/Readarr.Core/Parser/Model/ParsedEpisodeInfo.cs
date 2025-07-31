using System;
using System.Collections.Generic;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;
using Readarr.Core.Tv;

namespace Readarr.Core.Parser.Model
{
    // Stub class for TV compatibility - to be removed
    public class ParsedEpisodeInfo
    {
        public string SeriesTitle { get; set; }
        public string SeriesTitleInfo { get; set; }
        public int SeriesYear { get; set; }
        public int SeasonNumber { get; set; }
        public int[] EpisodeNumbers { get; set; }
        public int[] AbsoluteEpisodeNumbers { get; set; }
        public string AirDate { get; set; }
        public Language Language { get; set; }
        public bool FullSeason { get; set; }
        public bool IsPartialSeason { get; set; }
        public bool IsMultiSeason { get; set; }
        public bool IsSeasonExtra { get; set; }
        public bool Special { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseHash { get; set; }
        public string ReleaseTitle { get; set; }
        public string ReleaseTokens { get; set; }
        public QualityModel Quality { get; set; }
        public bool IsDaily => !string.IsNullOrWhiteSpace(AirDate);
        public bool IsAbsoluteNumbering => AbsoluteEpisodeNumbers != null && AbsoluteEpisodeNumbers.Length > 0;
        public bool IsPossibleSpecialEpisode => (EpisodeNumbers != null && EpisodeNumbers.Length == 1 && EpisodeNumbers[0] == 0) ||
                                                 (AbsoluteEpisodeNumbers != null && AbsoluteEpisodeNumbers.Length == 1 && AbsoluteEpisodeNumbers[0] == 0);
    }
}