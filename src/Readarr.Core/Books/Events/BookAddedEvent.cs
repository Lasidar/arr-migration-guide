using Readarr.Common.Messaging;

namespace Readarr.Core.Books.Events
{
    public class BookAddedEvent : IEvent
    {
        public Book Book { get; private set; }

        public BookAddedEvent(Book book)
        {
            Book = book;
        }
    }
}