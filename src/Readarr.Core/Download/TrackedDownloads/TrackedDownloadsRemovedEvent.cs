using System.Collections.Generic;
using Readarr.Common.Messaging;

namespace Readarr.Core.Download.TrackedDownloads
{
    public class TrackedDownloadsRemovedEvent : IEvent
    {
        public List<TrackedDownload> TrackedDownloads { get; private set; }

        public TrackedDownloadsRemovedEvent(List<TrackedDownload> trackedDownloads)
        {
            TrackedDownloads = trackedDownloads;
        }
    }
}
