using Readarr.Common.Messaging;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaCover
{
    public class MediaCoversUpdatedEvent : IEvent
    {
        public Series Series { get; set; }
        public bool Updated { get; set; }

        public MediaCoversUpdatedEvent(Series series, bool updated)
        {
            Series = series;
            Updated = updated;
        }
    }
}
