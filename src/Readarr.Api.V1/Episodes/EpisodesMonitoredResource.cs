using System.Collections.Generic;

namespace Readarr.Api.V3.Episodes
{
    public class EditionsMonitoredResource
    {
        public List<int> EpisodeIds { get; set; }
        public bool Monitored { get; set; }
    }
}
