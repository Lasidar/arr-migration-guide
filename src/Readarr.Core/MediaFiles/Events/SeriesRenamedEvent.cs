using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Books;

namespace Readarr.Core.MediaFiles.Events
{
    public class AuthorRenamedEvent : IEvent
    {
        public Series Series { get; private set; }
        public List<RenamedBookFile> RenamedFiles { get; private set; }

        public AuthorRenamedEvent(Series series, List<RenamedBookFile> renamedFiles)
        {
            Series = series;
            RenamedFiles = renamedFiles;
        }
    }
}
