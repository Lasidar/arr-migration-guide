using System.Collections.Generic;
using Readarr.Api.V3.Series;

namespace Readarr.Api.V3.SeasonPass
{
    public class BookPassSeriesResource
    {
        public int Id { get; set; }
        public bool? Monitored { get; set; }
        public List<SeasonResource> Seasons { get; set; }
    }
}
