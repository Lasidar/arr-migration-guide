using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(94)]
    public class add_tvmazeid : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Series").AddColumn("TvMazeId").AsInt32().WithDefaultValue(0);
            Create.Index().OnTable("Series").OnColumn("TvMazeId");
        }
    }
}
