using NzbDrone.Common.Messaging;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MediaCover
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
