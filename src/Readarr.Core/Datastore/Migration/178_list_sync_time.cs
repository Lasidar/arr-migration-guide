using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(178)]
    public class list_sync_time : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("LastSyncListInfo").FromTable("ImportListStatus");

            Alter.Table("ImportListStatus").AddColumn("LastInfoSync").AsDateTime().Nullable();
        }
    }
}
