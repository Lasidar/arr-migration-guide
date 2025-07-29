using System;
using System.Diagnostics;
using Readarr.Common.Disk;
using Readarr.Core.Indexers;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Download
{
    public class DownloadClientItem
    {
        public string DownloadId { get; set; }
        public string Title { get; set; }
        public long TotalSize { get; set; }
        public long RemainingSize { get; set; }
        public TimeSpan? RemainingTime { get; set; }
        public DateTime? CompletedTime { get; set; }
        public string OutputPath { get; set; }
        public string Category { get; set; }
        public DownloadItemStatus Status { get; set; }
        public bool IsEncrypted { get; set; }
        public bool CanMoveFiles { get; set; }
        public bool CanBeRemoved { get; set; }
        public bool Removed { get; set; }

        public DownloadClientInfo DownloadClientInfo { get; set; }
    }

    public class DownloadClientInfo
    {
        public DownloadProtocol Protocol { get; set; }
        public string Type { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum DownloadItemStatus
    {
        Queued,
        Paused,
        Downloading,
        Completed,
        Failed,
        Warning,
        Removed
    }

    public enum DownloadProtocol
    {
        Unknown = 0,
        Usenet = 1,
        Torrent = 2
    }

    public enum DownloadClientType
    {
        Usenet,
        Torrent,
        Pneumatic,
        Other
    }
}
