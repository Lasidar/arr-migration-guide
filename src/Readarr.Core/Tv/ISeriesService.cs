using System.Collections.Generic;

namespace Readarr.Core.Tv
{
    // Stub interface for TV compatibility - to be removed
    public interface ISeriesService
    {
        Series GetSeries(int seriesId);
        List<Series> GetAllSeries();
        Series AddSeries(Series newSeries);
        void UpdateSeries(Series series);
        void DeleteSeries(int seriesId, bool deleteFiles);
        List<Series> GetSeriesByTvdbId(int tvdbId);
        Series FindByTitle(string title);
        Series FindByTitleInexact(string title);
        void SetSeriesType(int seriesId, SeriesTypes seriesType);
    }
}