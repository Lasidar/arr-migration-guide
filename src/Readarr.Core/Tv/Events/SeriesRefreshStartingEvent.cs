using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    public class SeriesRefreshStartingEvent : IEvent
    {
        public bool ManualTrigger { get; set; }

        public SeriesRefreshStartingEvent(bool manualTrigger)
        {
            ManualTrigger = manualTrigger;
        }
    }
}
