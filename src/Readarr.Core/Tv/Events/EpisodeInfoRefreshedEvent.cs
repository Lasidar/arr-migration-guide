using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Tv;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class EpisodeInfoRefreshedEvent : IEvent
    {
        public Tv.Series Series { get; set; }
        public List<Episode> Added { get; set; }
        public List<Episode> Updated { get; set; }

        public EpisodeInfoRefreshedEvent(Tv.Series series, List<Episode> added, List<Episode> updated)
        {
            Series = series;
            Added = added;
            Updated = updated;
        }
    }
}