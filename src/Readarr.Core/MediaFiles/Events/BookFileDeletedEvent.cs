using Readarr.Common.Messaging;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookFileDeletedEvent : IEvent
    {
        public BookFile BookFile { get; private set; }
        public DeleteMediaFileReason Reason { get; private set; }

        public BookFileDeletedEvent(BookFile bookFile, DeleteMediaFileReason reason)
        {
            BookFile = bookFile;
            Reason = reason;
        }
    }
}