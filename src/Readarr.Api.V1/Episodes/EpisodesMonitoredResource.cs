using System.Collections.Generic;

namespace Readarr.Api.V1.Episodes
{
    public class EpisodesMonitoredResource
    {
        public List<int> EpisodeIds { get; set; }
        public bool Monitored { get; set; }
    }
}
