using Readarr.Core.Indexers;

namespace Readarr.Core.Housekeeping.Housekeepers
{
    public class FixFutureIndexerStatusTimes : FixFutureProviderStatusTimes<IndexerStatus>, IHousekeepingTask
    {
        public FixFutureIndexerStatusTimes(IIndexerStatusRepository indexerStatusRepository)
            : base(indexerStatusRepository)
        {
        }
    }
}
