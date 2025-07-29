using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(149)]
    public class add_on_delete_to_notifications : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Notifications").AddColumn("OnSeriesDelete").AsBoolean().WithDefaultValue(false);
            Alter.Table("Notifications").AddColumn("OnEpisodeFileDelete").AsBoolean().WithDefaultValue(false);
        }
    }
}
