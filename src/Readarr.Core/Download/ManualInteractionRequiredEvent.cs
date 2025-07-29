using Readarr.Common.Messaging;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.Download
{
    public class ManualInteractionRequiredEvent : IEvent
    {
        public RemoteEpisode Episode { get; private set; }
        public TrackedDownload TrackedDownload { get; private set; }
        public GrabbedReleaseInfo Release { get; private set; }

        public ManualInteractionRequiredEvent(TrackedDownload trackedDownload, GrabbedReleaseInfo release)
        {
            TrackedDownload = trackedDownload;
            Episode = trackedDownload.RemoteEpisode;
            Release = release;
        }
    }
}
