using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.Notifications
{
    public interface INotificationStatusRepository : IProviderStatusRepository<NotificationStatus>
    {
    }

    public class NotificationStatusRepository : ProviderStatusRepository<NotificationStatus>, INotificationStatusRepository
    {
        public NotificationStatusRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }
    }
}
