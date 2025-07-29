using System.Collections.Generic;
using NzbDrone.Common.Messaging;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MediaFiles.Events
{
    public class AuthorScannedEvent : IEvent
    {
        public Series Series { get; private set; }
        public List<string> PossibleExtraFiles { get; set; }

        public SeriesScannedEvent(Series series, List<string> possibleExtraFiles)
        {
            Series = series;
            PossibleExtraFiles = possibleExtraFiles;
        }
    }
}
