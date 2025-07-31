using Readarr.Core.Download;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Core.Books;

namespace Readarr.Core.Notifications
{
    public class ManualInteractionRequiredMessage
    {
        public string Message { get; set; }
        public Series Series { get; set; }
        public RemoteEpisode Episode { get; set; }
        public TrackedDownload TrackedDownload { get; set; }
        public QualityModel Quality { get; set; }
        public DownloadClientItemClientInfo DownloadClientInfo { get; set; }
        public string DownloadId { get; set; }
        public GrabbedReleaseInfo Release { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}
