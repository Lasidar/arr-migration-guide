using System.Collections.Generic;
using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.Books.Events
{
    public class SeriesImportedEvent : IEvent
    {
        public List<int> SeriesIds { get; private set; }

        public SeriesImportedEvent(List<int> seriesIds)
        {
            SeriesIds = seriesIds;
        }
    }
}
