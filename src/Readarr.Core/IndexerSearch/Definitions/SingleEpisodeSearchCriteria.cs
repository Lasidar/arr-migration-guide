using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.IndexerSearch.Definitions
{
    public class SingleEpisodeSearchCriteria : SearchCriteriaBase
    {
        public List<Episode> Episodes { get; set; }

        public override string ToString()
        {
            return "[Single Episode Search]";
        }
    }
}