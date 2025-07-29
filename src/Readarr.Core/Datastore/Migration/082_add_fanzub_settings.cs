using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(82)]
    public class add_fanzub_settings : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Update.Table("Indexers").Set(new { ConfigContract = "FanzubSettings" }).Where(new { Implementation = "Fanzub", ConfigContract = "NullConfig" });
        }
    }
}
