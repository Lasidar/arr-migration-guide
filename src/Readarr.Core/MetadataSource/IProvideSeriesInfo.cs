using System;
using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.MetadataSource
{
    public interface IProvideSeriesInfo
    {
        Tuple<Series, List<Episode>> GetSeriesInfo(int tvdbSeriesId);
    }
}
