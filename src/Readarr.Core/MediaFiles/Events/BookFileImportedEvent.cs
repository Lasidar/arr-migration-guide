using System.Collections.Generic;
using Readarr.Common.Messaging;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookFileImportedEvent : IEvent
    {
        public List<BookFile> ImportedBooks { get; private set; }
        public List<BookFile> OldFiles { get; private set; }
        public bool NewDownload { get; private set; }

        public BookFileImportedEvent(List<BookFile> importedBooks, List<BookFile> oldFiles, bool newDownload)
        {
            ImportedBooks = importedBooks;
            OldFiles = oldFiles;
            NewDownload = newDownload;
        }
    }
}