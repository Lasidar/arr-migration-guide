using Readarr.Common.Messaging;

namespace Readarr.Core.Books.Events
{
    public class AuthorAddedEvent : IEvent
    {
        public Author Author { get; private set; }

        public AuthorAddedEvent(Author author)
        {
            Author = author;
        }
    }
}