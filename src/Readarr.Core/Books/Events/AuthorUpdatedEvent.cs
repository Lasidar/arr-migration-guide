using Readarr.Common.Messaging;

namespace Readarr.Core.Books.Events
{
    public class AuthorUpdatedEvent : IEvent
    {
        public Author Author { get; private set; }

        public AuthorUpdatedEvent(Author author)
        {
            Author = author;
        }
    }
}