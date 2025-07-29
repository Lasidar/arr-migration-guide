using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(125)]
    public class remove_notify_my_android_and_pushalot_notifications : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.FromTable("Notifications").Row(new { Implementation = "NotifyMyAndroid" });
            Delete.FromTable("Notifications").Row(new { Implementation = "Pushalot" });
        }
    }
}
