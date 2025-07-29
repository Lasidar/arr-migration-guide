using Readarr.Common.Messaging;

namespace Readarr.Core.Books.Events
{
    public class AuthorRenamedEvent : IEvent
    {
        public Author Author { get; private set; }
        public string PreviousPath { get; private set; }

        public AuthorRenamedEvent(Author author, string previousPath)
        {
            Author = author;
            PreviousPath = previousPath;
        }
    }
}