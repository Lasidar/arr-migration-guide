using Readarr.Common.Messaging;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookFileRenamedEvent : IEvent
    {
        public BookFile BookFile { get; private set; }
        public string OriginalPath { get; private set; }

        public BookFileRenamedEvent(BookFile bookFile, string originalPath)
        {
            BookFile = bookFile;
            OriginalPath = originalPath;
        }
    }
}