using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(197)]
    public class list_add_missing_search : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("ImportLists").AddColumn("SearchForMissingBooks").AsBoolean().NotNullable().WithDefaultValue(true);
        }
    }
}
