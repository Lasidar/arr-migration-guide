using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(127)]
    public class rename_restrictions_to_release_profiles : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Rename.Table("Restrictions").To("ReleaseProfiles");
            Alter.Table("ReleaseProfiles").AddColumn("IncludePreferredWhenRenaming").AsBoolean().WithDefaultValue(true);
        }
    }
}
