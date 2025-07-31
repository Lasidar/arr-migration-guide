using Readarr.Common.Messaging;
using Readarr.Core.Tv;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class SeriesAddCompletedEvent : IEvent
    {
        public Tv.Series Series { get; private set; }

        public SeriesAddCompletedEvent(Tv.Series series)
        {
            Series = series;
        }
    }
}