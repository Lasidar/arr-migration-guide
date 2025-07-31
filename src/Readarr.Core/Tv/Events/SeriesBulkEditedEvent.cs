using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Tv;

namespace Readarr.Core.Tv.Events
{
    // Stub class for TV compatibility - to be removed
    public class SeriesBulkEditedEvent : IEvent
    {
        public List<Tv.Series> Series { get; set; }

        public SeriesBulkEditedEvent(List<Tv.Series> series)
        {
            Series = series;
        }
    }
}