using System.Collections.Generic;
using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    public class SeriesBulkEditedEvent : IEvent
    {
        public List<Series> Series { get; private set; }

        public SeriesBulkEditedEvent(List<Series> series)
        {
            Series = series;
        }
    }
}
