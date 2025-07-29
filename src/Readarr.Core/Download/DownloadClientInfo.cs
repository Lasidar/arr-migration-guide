using System.Collections.Generic;
using Readarr.Common.Disk;

namespace Readarr.Core.Download
{
    public class DownloadClientInfo
    {
        public DownloadClientInfo()
        {
            OutputRootFolders = new List<OsPath>();
        }

        public bool IsLocalhost { get; set; }
        public string SortingMode { get; set; }
        public bool RemovesCompletedDownloads { get; set; }
        public bool RemoveCompletedDownloads => RemovesCompletedDownloads; // Alias for compatibility
        public List<OsPath> OutputRootFolders { get; set; }
        public string Name { get; set; }
        public DownloadClientType Type { get; set; }
        public bool HasPostImportCategory { get; set; }
    }
}
