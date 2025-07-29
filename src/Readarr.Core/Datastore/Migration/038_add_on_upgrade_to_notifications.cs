using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(38)]
    public class add_on_upgrade_to_notifications : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Notifications").AddColumn("OnUpgrade").AsBoolean().Nullable();

            Execute.Sql("UPDATE \"Notifications\" SET \"OnUpgrade\" = \"OnDownload\"");
        }
    }
}
