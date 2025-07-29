using System.Collections.Generic;

namespace NzbDrone.Core.MediaFiles
{
    public class RenameEditionFilePreview
    {
        public int AuthorId { get; set; }
        public int BookNumber { get; set; }
        public List<int> EditionNumbers { get; set; }
        public int EditionFileId { get; set; }
        public string ExistingPath { get; set; }
        public string NewPath { get; set; }
    }
}
