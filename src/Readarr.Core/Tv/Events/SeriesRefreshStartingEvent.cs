using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class SeriesRefreshStartingEvent : IEvent
    {
        public bool ManualTrigger { get; set; }

        public SeriesRefreshStartingEvent(bool manualTrigger)
        {
            ManualTrigger = manualTrigger;
        }
    }
}