using System.Collections.Generic;
using Readarr.Common.Messaging;

namespace Readarr.Core.Books.Events
{
    public class AuthorsImportedEvent : IEvent
    {
        public List<int> AuthorIds { get; private set; }

        public AuthorsImportedEvent(List<int> authorIds)
        {
            AuthorIds = authorIds;
        }
    }
}