using System.Collections.Generic;
using Readarr.Common.Messaging;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles.Events
{
    public class AuthorRenamedEvent : IEvent
    {
        public Tv.Series Series { get; private set; }
        public List<RenamedBookFile> RenamedFiles { get; private set; }

        public AuthorRenamedEvent(Tv.Series series, List<RenamedBookFile> renamedFiles)
        {
            Series = series;
            RenamedFiles = renamedFiles;
        }
    }
}
