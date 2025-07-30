using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Books;
using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookImportedEvent : IEvent
    {
        public LocalBook BookInfo { get; private set; }
        public BookFile ImportedBook { get; private set; }
        public List<BookFile> OldFiles { get; private set; }
        public bool NewDownload { get; private set; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; set; }
        public string DownloadId { get; private set; }

        public BookImportedEvent(LocalBook bookInfo, BookFile importedBook, List<BookFile> oldFiles, bool newDownload, DownloadClientItemClientInfo downloadClientInfo, string downloadId)
        {
            BookInfo = bookInfo;
            ImportedBook = importedBook;
            OldFiles = oldFiles;
            NewDownload = newDownload;
            DownloadClientInfo = downloadClientInfo;
            DownloadId = downloadId;
        }
    }
}