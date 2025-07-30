using System.Collections.Generic;
using Readarr.Core.Books;

namespace Readarr.Core.Notifications
{
    public class AuthorAddMessage
    {
        public string Message { get; set; }
        public Author Author { get; set; }
        public List<Book> Books { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}