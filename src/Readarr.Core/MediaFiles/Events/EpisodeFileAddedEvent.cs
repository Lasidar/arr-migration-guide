using Readarr.Common.Messaging;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.Events
{
    // Stub class for TV compatibility - to be removed
    public class EpisodeFileAddedEvent : IEvent
    {
        public EpisodeFile EpisodeFile { get; private set; }

        public EpisodeFileAddedEvent(EpisodeFile episodeFile)
        {
            EpisodeFile = episodeFile;
        }
    }
}
