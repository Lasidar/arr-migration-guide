using System.Collections.Generic;
using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.Books.Events
{
    public class AuthorBulkEditedEvent : IEvent
    {
        public List<Series> Series { get; private set; }

        public SeriesBulkEditedEvent(List<Series> series)
        {
            Series = series;
        }
    }
}
