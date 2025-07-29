using System.Collections.Generic;
using NzbDrone.Common.Messaging;

namespace NzbDrone.Core.Books.Events
{
    public class AuthorImportedEvent : IEvent
    {
        public List<int> AuthorIds { get; private set; }

        public SeriesImportedEvent(List<int> seriesIds)
        {
            AuthorIds = seriesIds;
        }
    }
}
