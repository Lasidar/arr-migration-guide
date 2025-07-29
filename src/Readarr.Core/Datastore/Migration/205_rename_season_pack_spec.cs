using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(205)]
    public class rename_season_pack_spec : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Execute.Sql("UPDATE \"CustomFormats\" SET \"Specifications\" = REPLACE(\"Specifications\", 'SeasonPackSpecification', 'ReleaseTypeSpecification')");
        }
    }
}
