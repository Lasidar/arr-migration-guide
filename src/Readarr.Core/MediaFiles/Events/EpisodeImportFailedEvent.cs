using System;
using Readarr.Common.Messaging;
using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.Events
{
    // Stub class for TV compatibility - to be removed
    public class EpisodeImportFailedEvent : IEvent
    {
        public Exception Exception { get; set; }
        public LocalEpisode EpisodeInfo { get; }
        public bool NewDownload { get; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; }
        public string DownloadId { get; }

        public EpisodeImportFailedEvent(Exception exception, LocalEpisode episodeInfo, bool newDownload, DownloadClientItemClientInfo downloadClientInfo, string downloadId)
        {
            Exception = exception;
            EpisodeInfo = episodeInfo;
            NewDownload = newDownload;
            DownloadClientInfo = downloadClientInfo;
            DownloadId = downloadId;
        }
    }
}