using System.Collections.Generic;
using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
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
