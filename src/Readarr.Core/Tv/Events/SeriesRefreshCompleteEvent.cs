using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class SeriesRefreshCompleteEvent : IEvent
    {
        public Tv.Series Series { get; set; }

        public SeriesRefreshCompleteEvent(Tv.Series series)
        {
            Series = series;
        }
    }
}