using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.Books.Events
{
    public class AuthorAddedEvent : IEvent
    {
        public Series Series { get; private set; }

        public SeriesAddedEvent(Series series)
        {
            Series = series;
        }
    }
}
