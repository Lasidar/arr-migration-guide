using System.Collections.Generic;
using Readarr.Common.Messaging;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class SeriesBulkEditedEvent : IEvent
    {
        public List<Series> Series { get; set; }

        public SeriesBulkEditedEvent(List<Series> series)
        {
            Series = series;
        }
    }
}