using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.CustomFormats
{
    public interface ICustomFormatRepository : IBasicRepository<CustomFormat>
    {
    }

    public class CustomFormatRepository : BasicRepository<CustomFormat>, ICustomFormatRepository
    {
        public CustomFormatRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
