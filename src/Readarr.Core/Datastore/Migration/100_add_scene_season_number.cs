using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(100)]
    public class add_scene_season_number : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("SceneMappings").AlterColumn("SeasonNumber").AsInt32().Nullable();
            Alter.Table("SceneMappings").AddColumn("SceneSeasonNumber").AsInt32().Nullable();
        }
    }
}
