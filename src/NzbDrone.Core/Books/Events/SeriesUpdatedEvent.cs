using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.Books.Events
{
    public class AuthorUpdatedEvent : IEvent
    {
        public Series Series { get; private set; }

        public SeriesUpdatedEvent(Series series)
        {
            Series = series;
        }
    }
}
