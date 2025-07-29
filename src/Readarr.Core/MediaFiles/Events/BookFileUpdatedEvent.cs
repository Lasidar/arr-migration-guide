using Readarr.Common.Messaging;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookFileUpdatedEvent : IEvent
    {
        public Book Book { get; private set; }

        public BookFileUpdatedEvent(Book book)
        {
            Book = book;
        }
    }
}