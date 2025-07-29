using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Api.V1.SeasonPass
{
    public class SeasonPassResource
    {
        public List<SeasonPassSeriesResource> Series { get; set; }
        public MonitoringOptions MonitoringOptions { get; set; }
    }
}
