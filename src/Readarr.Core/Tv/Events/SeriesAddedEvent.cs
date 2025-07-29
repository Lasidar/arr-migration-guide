using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    public class SeriesAddedEvent : IEvent
    {
        public Series Series { get; private set; }

        public SeriesAddedEvent(Series series)
        {
            Series = series;
        }
    }
}
