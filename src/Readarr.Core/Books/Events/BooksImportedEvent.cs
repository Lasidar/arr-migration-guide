using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Messaging;
using Readarr.Core.Download;
using Readarr.Core.MediaFiles;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.Books.Events
{
    public class BooksImportedEvent : IEvent
    {
        public Author Author { get; private set; }
        public List<Book> Books { get; private set; }
        public List<BookFile> BookFiles { get; private set; }
        public List<BookFile> OldFiles { get; private set; }
        public bool NewDownload { get; private set; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; set; }
        public string DownloadId { get; private set; }

        public BooksImportedEvent(Author author, List<MediaFiles.BookImport.ImportDecision<Parser.Model.LocalBook>> importedBooks, bool newDownload, DownloadClientItemClientInfo downloadClientInfo, string downloadId)
        {
            Author = author;
            Books = importedBooks.Select(d => d.Item.Book).ToList();
            BookFiles = importedBooks.Select(d => d.Item.BookFile).ToList();
            OldFiles = importedBooks.Select(d => d.Item.OldFiles).SelectMany(f => f ?? new List<BookFile>()).ToList();
            NewDownload = newDownload;
            DownloadClientInfo = downloadClientInfo;
            DownloadId = downloadId;
        }
    }
}