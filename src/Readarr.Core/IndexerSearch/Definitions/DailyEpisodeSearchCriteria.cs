using System;

namespace Readarr.Core.IndexerSearch.Definitions
{
    public class DailyEpisodeSearchCriteria : SearchCriteriaBase
    {
        public DateTime AirDate { get; set; }

        public override string ToString()
        {
            return "[Daily Episode Search]";
        }
    }
}