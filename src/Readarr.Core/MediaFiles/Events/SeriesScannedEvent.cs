using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.Events
{
    public class SeriesScannedEvent : IEvent
    {
        public Tv.Series Series { get; private set; }
        public List<string> PossibleExtraFiles { get; set; }

        public SeriesScannedEvent(Tv.Series series, List<string> possibleExtraFiles)
        {
            Series = series;
            PossibleExtraFiles = possibleExtraFiles;
        }
    }
}
