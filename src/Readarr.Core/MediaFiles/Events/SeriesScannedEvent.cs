using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles.Events
{
    public class SeriesScannedEvent : IEvent
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
