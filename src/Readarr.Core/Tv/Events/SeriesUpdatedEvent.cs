using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    public class SeriesUpdatedEvent : IEvent
    {
        public Series Series { get; private set; }

        public SeriesUpdatedEvent(Series series)
        {
            Series = series;
        }
    }
}
