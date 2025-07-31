using Readarr.Common.Messaging;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.Events
{
    // Stub class for TV compatibility - to be removed
    public class EpisodeGrabbedEvent : IEvent
    {
        public RemoteEpisode Episode { get; private set; }

        public EpisodeGrabbedEvent(RemoteEpisode episode)
        {
            Episode = episode;
        }
    }
}