using System.Collections.Generic;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.IndexerSearch
{
    public class EpisodeSearchGroup
    {
        public int SeriesId { get; set; }
        public int SeasonNumber { get; set; }
        public List<Episode> Episodes { get; set; }
    }
}
