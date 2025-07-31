using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.Tv
{
    // Stub interface for TV compatibility - to be removed
    public interface IAddSeriesService
    {
        Series AddSeries(Series newSeries);
        List<Tv.Series> AddSeries(List<Tv.Series> newSeries);
    }
}