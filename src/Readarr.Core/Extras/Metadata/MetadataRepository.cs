using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Extras.Metadata
{
    public interface IMetadataRepository : IProviderRepository<MetadataDefinition>
    {
    }

    public class MetadataRepository : ProviderRepository<MetadataDefinition>, IMetadataRepository
    {
        public MetadataRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
