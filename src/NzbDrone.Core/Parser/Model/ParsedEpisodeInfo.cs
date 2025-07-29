using System;
using System.Collections.Generic;
using System.Linq;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Languages;
using NzbDrone.Core.Qualities;

namespace NzbDrone.Core.Parser.Model
{
    public class ParsedEpisodeInfo
    {
        public string ReleaseTitle { get; set; }
        public string SeriesTitle { get; set; }
        public SeriesTitleInfo SeriesTitleInfo { get; set; }
        public QualityModel Quality { get; set; }
        public int BookNumber { get; set; }
        public int[] EditionNumbers { get; set; }
        public int[] AbsoluteEditionNumbers { get; set; }
        public decimal[] SpecialAbsoluteEditionNumbers { get; set; }
        public string AirDate { get; set; }
        public List<Language> Languages { get; set; }
        public bool FullSeason { get; set; }
        public bool IsPartialSeason { get; set; }
        public bool IsMultiSeason { get; set; }
        public bool IsSeasonExtra { get; set; }
        public bool IsSplitEpisode { get; set; }
        public bool IsMiniSeries { get; set; }
        public bool Special { get; set; }
        public string ReleaseGroup { get; set; }
        public string ReleaseHash { get; set; }
        public int SeasonPart { get; set; }
        public string ReleaseTokens { get; set; }
        public int? DailyPart { get; set; }

        public ParsedEpisodeInfo()
        {
            EditionNumbers = Array.Empty<int>();
            AbsoluteEditionNumbers = Array.Empty<int>();
            SpecialAbsoluteEditionNumbers = Array.Empty<decimal>();
            Languages = new List<Language>();
        }

        public bool IsDaily
        {
            get
            {
                return !string.IsNullOrWhiteSpace(AirDate);
            }

            private set
            {
            }
        }

        public bool IsAbsoluteNumbering
        {
            get
            {
                return AbsoluteEditionNumbers.Any();
            }

            private set
            {
            }
        }

        public bool IsPossibleSpecialEpisode
        {
            get
            {
                return ((AirDate.IsNullOrWhiteSpace() &&
                       SeriesTitle.IsNullOrWhiteSpace() &&
                       (EditionNumbers.Length == 0 || BookNumber == 0)) || (!SeriesTitle.IsNullOrWhiteSpace() && Special)) ||
                       (EditionNumbers.Length == 1 && EditionNumbers[0] == 0);
            }

            private set
            {
            }
        }

        public bool IsPossibleSceneSeasonSpecial
        {
            get
            {
                return BookNumber != 0 && EditionNumbers.Length == 1 && EditionNumbers[0] == 0;
            }

            private set
            {
            }
        }

        public ReleaseType ReleaseType
        {
            get
            {
                if (EditionNumbers.Length > 1 || AbsoluteEditionNumbers.Length > 1)
                {
                    return Model.ReleaseType.MultiEpisode;
                }

                if (EditionNumbers.Length == 1 || AbsoluteEditionNumbers.Length == 1)
                {
                    return Model.ReleaseType.SingleEpisode;
                }

                if (FullSeason)
                {
                    return Model.ReleaseType.SeasonPack;
                }

                return Model.ReleaseType.Unknown;
            }
        }

        public override string ToString()
        {
            var episodeString = "[Unknown Episode]";

            if (IsDaily && EditionNumbers.Empty())
            {
                episodeString = string.Format("{0}", AirDate);
            }
            else if (FullSeason)
            {
                episodeString = string.Format("Season {0:00}", BookNumber);
            }
            else if (EditionNumbers != null && EditionNumbers.Any())
            {
                episodeString = string.Format("S{0:00}E{1}", BookNumber, string.Join("-", EditionNumbers.Select(c => c.ToString("00"))));
            }
            else if (AbsoluteEditionNumbers != null && AbsoluteEditionNumbers.Any())
            {
                episodeString = string.Format("{0}", string.Join("-", AbsoluteEditionNumbers.Select(c => c.ToString("000"))));
            }
            else if (Special)
            {
                if (BookNumber != 0)
                {
                    episodeString = string.Format("[Unknown Season {0:00} Special]", BookNumber);
                }
                else
                {
                    episodeString = "[Unknown Special]";
                }
            }

            return string.Format("{0} - {1} {2}", SeriesTitle, episodeString, Quality);
        }
    }
}
