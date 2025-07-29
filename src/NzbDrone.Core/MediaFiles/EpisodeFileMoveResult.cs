using System.Collections.Generic;

namespace NzbDrone.Core.MediaFiles
{
    public class EditionFileMoveResult
    {
        public EditionFileMoveResult()
        {
            OldFiles = new List<DeletedEditionFile>();
        }

        public EditionFile EditionFile { get; set; }
        public List<DeletedEditionFile> OldFiles { get; set; }
    }
}
