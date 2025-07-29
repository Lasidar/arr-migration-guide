using System;
using System.Collections.Generic;
using NzbDrone.Core.SeriesStats;

namespace Readarr.Api.V3.Series
{
    public class BookStatisticsResource
    {
        public DateTime? NextAiring { get; set; }
        public DateTime? PreviousAiring { get; set; }
        public int EditionFileCount { get; set; }
        public int EpisodeCount { get; set; }
        public int TotalEpisodeCount { get; set; }
        public long SizeOnDisk { get; set; }
        public List<string> ReleaseGroups { get; set; }

        public decimal PercentOfEpisodes
        {
            get
            {
                if (EpisodeCount == 0)
                {
                    return 0;
                }

                return (decimal)EditionFileCount / (decimal)EpisodeCount * 100;
            }
        }
    }

    public static class BookStatisticsResourceMapper
    {
        public static SeasonStatisticsResource ToResource(this SeasonStatistics model)
        {
            if (model == null)
            {
                return null;
            }

            return new SeasonStatisticsResource
            {
                NextAiring = model.NextAiring,
                PreviousAiring = model.PreviousAiring,
                EditionFileCount = model.EditionFileCount,
                EpisodeCount = model.EpisodeCount,
                TotalEpisodeCount = model.TotalEpisodeCount,
                SizeOnDisk = model.SizeOnDisk,
                ReleaseGroups = model.ReleaseGroups
            };
        }
    }
}
