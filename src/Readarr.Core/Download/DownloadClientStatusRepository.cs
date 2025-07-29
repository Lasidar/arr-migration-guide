using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.Download
{
    public interface IDownloadClientStatusRepository : IProviderStatusRepository<DownloadClientStatus>
    {
    }

    public class DownloadClientStatusRepository : ProviderStatusRepository<DownloadClientStatus>, IDownloadClientStatusRepository
    {
        public DownloadClientStatusRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
