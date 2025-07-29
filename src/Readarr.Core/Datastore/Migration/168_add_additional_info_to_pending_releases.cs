using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(168)]
    public class add_additional_info_to_pending_releases : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("PendingReleases").AddColumn("AdditionalInfo").AsString().Nullable();
        }
    }
}
