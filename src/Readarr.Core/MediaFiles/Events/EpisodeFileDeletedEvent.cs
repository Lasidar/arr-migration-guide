using Readarr.Common.Messaging;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.Events
{
    // Stub class for TV compatibility - to be removed
    public class EpisodeFileDeletedEvent : IEvent
    {
        public EpisodeFile EpisodeFile { get; private set; }
        public Tv.Series Series { get; private set; }
        public bool Reason { get; private set; }

        public EpisodeFileDeletedEvent(EpisodeFile episodeFile, Tv.Series series, bool reason)
        {
            EpisodeFile = episodeFile;
            Series = series;
            Reason = reason;
        }
    }
}
