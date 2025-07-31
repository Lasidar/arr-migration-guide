using Readarr.Common.Messaging;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.Events
{
    public class EpisodeFileRenamedEvent : IEvent
    {
        public Tv.Series Series { get; private set; }
        public EpisodeFile EpisodeFile { get; private set; }
        public string OriginalPath { get; private set; }

        public EpisodeFileRenamedEvent(Tv.Series series, EpisodeFile episodeFile, string originalPath)
        {
            Series = series;
            EpisodeFile = episodeFile;
            OriginalPath = originalPath;
        }
    }
}
