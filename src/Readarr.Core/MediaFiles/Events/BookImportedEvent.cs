using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Download;
using Readarr.Core.MediaFiles.BookImport;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookImportedEvent : IEvent
    {
        public LocalBook BookInfo { get; private set; }
        public BookFile ImportedBook { get; private set; }
        public List<BookFile> OldFiles { get; private set; }
        public bool NewDownload { get; private set; }
        public DownloadClientItem DownloadClientInfo { get; set; }
        public string DownloadId { get; private set; }

        public BookImportedEvent(LocalBook bookInfo, BookFile importedBook, List<BookFile> oldFiles, bool newDownload, DownloadClientItem downloadClientItem)
        {
            BookInfo = bookInfo;
            ImportedBook = importedBook;
            OldFiles = oldFiles;
            NewDownload = newDownload;
            DownloadClientInfo = downloadClientItem;
            DownloadId = downloadClientItem?.DownloadId;
        }
    }
}