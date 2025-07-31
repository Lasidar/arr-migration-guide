using Readarr.Common.Messaging;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles.Events
{
    public class EpisodeFolderCreatedEvent : IEvent
    {
        public Series Series { get; private set; }
        public EpisodeFile EpisodeFile { get; private set; }
        public string SeriesFolder { get; set; }
        public string SeasonFolder { get; set; }
        public string EpisodeFolder { get; set; }

        public EpisodeFolderCreatedEvent(Series series, EpisodeFile episodeFile)
        {
            Series = series;
            EpisodeFile = episodeFile;
        }
    }
}
