using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.IndexerSearch.Definitions
{
    public class SpecialEpisodeSearchCriteria : SearchCriteriaBase
    {
        public List<Episode> Episodes { get; set; }

        public override string ToString()
        {
            return "[Special Episode Search]";
        }
    }
}