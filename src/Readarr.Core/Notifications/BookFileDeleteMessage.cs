using Readarr.Core.Books;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Notifications
{
    public class BookFileDeleteMessage
    {
        public string Message { get; set; }
        public Author Author { get; set; }
        public Book Book { get; set; }
        public BookFile BookFile { get; set; }
        public DeleteMediaFileReason Reason { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}