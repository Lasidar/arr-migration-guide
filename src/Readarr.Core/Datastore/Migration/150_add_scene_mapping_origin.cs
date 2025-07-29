using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(150)]
    public class add_scene_mapping_origin : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("SceneMappings")
                .AddColumn("SceneOrigin").AsString().Nullable()
                .AddColumn("SearchMode").AsInt32().Nullable()
                .AddColumn("Comment").AsString().Nullable();
        }
    }
}
