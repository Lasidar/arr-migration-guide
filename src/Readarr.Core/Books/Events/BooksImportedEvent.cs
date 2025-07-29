using System.Collections.Generic;
using Readarr.Common.Messaging;

namespace Readarr.Core.Books.Events
{
    public class BooksImportedEvent : IEvent
    {
        public List<int> BookIds { get; private set; }

        public BooksImportedEvent(List<int> bookIds)
        {
            BookIds = bookIds;
        }
    }
}