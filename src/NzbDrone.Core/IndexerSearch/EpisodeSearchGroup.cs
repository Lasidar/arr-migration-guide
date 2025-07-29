using System.Collections.Generic;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.IndexerSearch
{
    public class EditionSearchGroup
    {
        public int AuthorId { get; set; }
        public int BookNumber { get; set; }
        public List<Episode> Episodes { get; set; }
    }
}
