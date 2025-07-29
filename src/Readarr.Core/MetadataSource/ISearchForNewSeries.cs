using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.MetadataSource
{
    public interface ISearchForNewSeries
    {
        List<Series> SearchForNewSeries(string title);
        List<Series> SearchForNewSeriesByImdbId(string imdbId);
        List<Series> SearchForNewSeriesByAniListId(int aniListId);
        List<Series> SearchForNewSeriesByTmdbId(int tmdbId);
        List<Series> SearchForNewSeriesByMyAnimeListId(int malId);
    }
}
