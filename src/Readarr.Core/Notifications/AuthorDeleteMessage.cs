using System.Collections.Generic;
using Readarr.Core.Books;

namespace Readarr.Core.Notifications
{
    public class AuthorDeleteMessage
    {
        public string Message { get; set; }
        public Author Author { get; set; }
        public bool DeletedFiles { get; set; }
        public List<string> DeletedFilePaths { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}