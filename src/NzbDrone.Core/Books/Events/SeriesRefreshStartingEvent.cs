using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.Books.Events
{
    public class AuthorRefreshStartingEvent : IEvent
    {
        public bool ManualTrigger { get; set; }

        public SeriesRefreshStartingEvent(bool manualTrigger)
        {
            ManualTrigger = manualTrigger;
        }
    }
}
