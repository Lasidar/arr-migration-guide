using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.ImportLists
{
    public interface IImportListStatusRepository : IProviderStatusRepository<ImportListStatus>
    {
    }

    public class ImportListStatusRepository : ProviderStatusRepository<ImportListStatus>, IImportListStatusRepository
    {
        public ImportListStatusRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
