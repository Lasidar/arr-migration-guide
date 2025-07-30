using System;
using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;

namespace Readarr.Core.History
{
    public class BookHistory : ModelBase
    {
        public BookHistory()
        {
            Data = new Dictionary<string, string>();
        }

        public int BookId { get; set; }
        public int AuthorId { get; set; }
        public string SourceTitle { get; set; }
        public QualityModel Quality { get; set; }
        public DateTime Date { get; set; }
        public Book Book { get; set; }
        public Author Author { get; set; }
        public HistoryEventType EventType { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public Language Language { get; set; }
        public string DownloadId { get; set; }
    }

    public enum HistoryEventType
    {
        Unknown = 0,
        Grabbed = 1,
        AuthorFolderImported = 2,
        DownloadFolderImported = 3,
        DownloadFailed = 4,
        BookFileDeleted = 5,
        BookFolderImported = 6,
        BookFileRenamed = 7,
        BookImportIncomplete = 8,
        DownloadImported = 9,
        BookFileRetagged = 10,
        DownloadIgnored = 11
    }
}