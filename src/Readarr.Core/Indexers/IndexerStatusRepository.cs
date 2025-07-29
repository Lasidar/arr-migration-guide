using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.Indexers
{
    public interface IIndexerStatusRepository : IProviderStatusRepository<IndexerStatus>
    {
    }

    public class IndexerStatusRepository : ProviderStatusRepository<IndexerStatus>, IIndexerStatusRepository
    {
        public IndexerStatusRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
