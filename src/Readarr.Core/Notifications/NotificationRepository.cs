using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Notifications
{
    public interface INotificationRepository : IProviderRepository<NotificationDefinition>
    {
        void UpdateSettings(NotificationDefinition model);
    }

    public class NotificationRepository : ProviderRepository<NotificationDefinition>, INotificationRepository
    {
        public NotificationRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public void UpdateSettings(NotificationDefinition model)
        {
            SetFields(model, m => m.Settings);
        }
    }
}
