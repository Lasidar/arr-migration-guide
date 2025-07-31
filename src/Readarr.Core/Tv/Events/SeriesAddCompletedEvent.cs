using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class SeriesAddCompletedEvent : IEvent
    {
        public Series Series { get; private set; }

        public SeriesAddCompletedEvent(Series series)
        {
            Series = series;
        }
    }
}