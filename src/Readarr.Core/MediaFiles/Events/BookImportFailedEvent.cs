using System;
using Readarr.Common.Messaging;
using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.Events
{
    public class BookImportFailedEvent : IEvent
    {
        public Exception Exception { get; set; }
        public LocalBook BookInfo { get; }
        public bool NewDownload { get; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; }
        public string DownloadId { get; }

        public BookImportFailedEvent(Exception exception, LocalBook bookInfo, bool newDownload, DownloadClientItemClientInfo downloadClientInfo, string downloadId)
        {
            Exception = exception;
            BookInfo = bookInfo;
            NewDownload = newDownload;
            DownloadClientInfo = downloadClientInfo;
            DownloadId = downloadId;
        }
    }
}