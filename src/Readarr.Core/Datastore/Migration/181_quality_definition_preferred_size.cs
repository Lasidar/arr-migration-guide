using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(181)]
    public class quality_definition_preferred_size : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("QualityDefinitions").AddColumn("PreferredSize").AsDouble().Nullable();

            Execute.Sql("UPDATE \"QualityDefinitions\" SET \"PreferredSize\" = \"MaxSize\" - 5 WHERE \"MaxSize\" > 5");
        }
    }
}
