using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(186)]
    public class add_result_to_commands : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Commands").AddColumn("Result").AsInt32().WithDefaultValue(1);
        }
    }
}
