using System;
using System.Collections.Generic;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MetadataSource
{
    public interface IProvideSeriesInfo
    {
        Tuple<Series, List<Episode>> GetSeriesInfo(int tvdbSeriesId);
    }
}
