using NzbDrone.Core.SeriesStats;

namespace Readarr.Api.V5.Series;

public class AuthorStatisticsResource
{
    public int SeasonCount { get; set; }
    public int EditionFileCount { get; set; }
    public int EpisodeCount { get; set; }
    public int TotalEpisodeCount { get; set; }
    public long SizeOnDisk { get; set; }
    public List<string>? ReleaseGroups { get; set; }

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

public static class AuthorStatisticsResourceMapper
{
    public static SeriesStatisticsResource ToResource(this SeriesStatistics model, List<SeasonResource>? seasons)
    {
        return new SeriesStatisticsResource
        {
            SeasonCount = seasons?.Count(s => s.BookNumber > 0) ?? 0,
            EditionFileCount = model.EditionFileCount,
            EpisodeCount = model.EpisodeCount,
            TotalEpisodeCount = model.TotalEpisodeCount,
            SizeOnDisk = model.SizeOnDisk,
            ReleaseGroups = model.ReleaseGroups
        };
    }
}
