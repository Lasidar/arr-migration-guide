using NzbDrone.Common.Messaging;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MediaFiles.Events
{
    public class EditionFileRenamedEvent : IEvent
    {
        public Series Series { get; private set; }
        public EditionFile EditionFile { get; private set; }
        public string OriginalPath { get; private set; }

        public EditionFileRenamedEvent(Series series, EditionFile episodeFile, string originalPath)
        {
            Series = series;
            EditionFile = episodeFile;
            OriginalPath = originalPath;
        }
    }
}
