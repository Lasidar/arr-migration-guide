using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.AutoTagging
{
    public interface IAutoTaggingRepository : IBasicRepository<AutoTag>
    {
    }

    public class AutoTaggingRepository : BasicRepository<AutoTag>, IAutoTaggingRepository
    {
        public AutoTaggingRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
