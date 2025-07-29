using Readarr.Core.Notifications;

namespace Readarr.Core.Housekeeping.Housekeepers
{
    public class FixFutureNotificationStatusTimes : FixFutureProviderStatusTimes<NotificationStatus>, IHousekeepingTask
    {
        public FixFutureNotificationStatusTimes(INotificationStatusRepository notificationStatusRepository)
            : base(notificationStatusRepository)
        {
        }
    }
}
