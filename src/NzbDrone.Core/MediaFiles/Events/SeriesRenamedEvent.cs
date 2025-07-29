using System.Collections.Generic;
using NzbDrone.Common.Messaging;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MediaFiles.Events
{
    public class AuthorRenamedEvent : IEvent
    {
        public Series Series { get; private set; }
        public List<RenamedEditionFile> RenamedFiles { get; private set; }

        public SeriesRenamedEvent(Series series, List<RenamedEditionFile> renamedFiles)
        {
            Series = series;
            RenamedFiles = renamedFiles;
        }
    }
}
