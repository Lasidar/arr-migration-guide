using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.Tv
{
    // Stub interface for TV compatibility - to be removed
    public interface ISeriesService
    {
        Series GetSeries(int seriesId);
        List<Tv.Series> GetAllSeries();
        Series AddSeries(Series newSeries);
        void UpdateSeries(Tv.Series series);
        void DeleteSeries(int seriesId, bool deleteFiles);
        List<Tv.Series> GetSeriesByTvdbId(int tvdbId);
        Series FindByTitle(string title);
        Series FindByTitleInexact(string title);
        void SetSeriesType(int seriesId, SeriesTypes seriesType);
    }
}