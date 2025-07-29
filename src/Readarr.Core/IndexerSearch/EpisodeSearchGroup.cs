using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.IndexerSearch
{
    public class EpisodeSearchGroup
    {
        public int SeriesId { get; set; }
        public int SeasonNumber { get; set; }
        public List<Episode> Episodes { get; set; }
    }
}
