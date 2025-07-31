using System.Collections.Generic;

namespace Readarr.Core.Tv
{
    // Stub interface for TV compatibility - to be removed
    public interface IAddSeriesService
    {
        Series AddSeries(Series newSeries);
        List<Series> AddSeries(List<Series> newSeries);
    }
}