using NzbDrone.Common.Messaging;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MediaFiles.Events
{
    public class EditionFolderCreatedEvent : IEvent
    {
        public Series Series { get; private set; }
        public EditionFile EditionFile { get; private set; }
        public string SeriesFolder { get; set; }
        public string SeasonFolder { get; set; }
        public string EpisodeFolder { get; set; }

        public EpisodeFolderCreatedEvent(Series series, EditionFile episodeFile)
        {
            Series = series;
            EditionFile = episodeFile;
        }
    }
}
