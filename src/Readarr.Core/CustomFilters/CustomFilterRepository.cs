using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.CustomFilters
{
    public interface ICustomFilterRepository : IBasicRepository<CustomFilter>
    {
    }

    public class CustomFilterRepository : BasicRepository<CustomFilter>, ICustomFilterRepository
    {
        public CustomFilterRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
