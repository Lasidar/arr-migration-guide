using System.Collections.Generic;

namespace Readarr.Core.Download.Clients.DownloadStation.Responses
{
    public class FileStationListResponse
    {
        public List<FileStationListFileInfoResponse> Files { get; set; }
    }
}
