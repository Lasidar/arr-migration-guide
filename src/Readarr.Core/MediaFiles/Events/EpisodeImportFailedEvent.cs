using System;
using Readarr.Common.Messaging;
using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.Events
{
    public class EpisodeImportFailedEvent : IEvent
    {
        public Exception Exception { get; set; }
        public LocalEpisode EpisodeInfo { get; }
        public bool NewDownload { get; }
        public DownloadClientItemClientInfo DownloadClientInfo { get;  }
        public string DownloadId { get; }

        public EpisodeImportFailedEvent(Exception exception, LocalEpisode episodeInfo, bool newDownload, DownloadClientItem downloadClientItem)
        {
            Exception = exception;
            EpisodeInfo = episodeInfo;
            NewDownload = newDownload;

            if (downloadClientItem != null)
            {
                DownloadClientInfo = new DownloadClientItemClientInfo
                {
                    Protocol = downloadClientItem.DownloadClientInfo.Type == DownloadClientType.Usenet ? DownloadProtocol.Usenet : DownloadProtocol.Torrent,
                    Type = downloadClientItem.DownloadClientInfo.Type.ToString(),
                    Id = 0, // TODO: Get actual download client ID
                    Name = downloadClientItem.DownloadClientInfo.Name
                };
                DownloadId = downloadClientItem.DownloadId;
            }
        }
    }
}
