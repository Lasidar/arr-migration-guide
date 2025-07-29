using System;
using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(65)]
    public class make_scene_numbering_nullable : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Update.Table("Episodes").Set(new { AbsoluteEditionNumber = DBNull.Value }).Where(new { AbsoluteEditionNumber = 0 });
            Update.Table("Episodes").Set(new { SceneAbsoluteEditionNumber = DBNull.Value }).Where(new { SceneAbsoluteEditionNumber = 0 });
            Update.Table("Episodes").Set(new { SceneBookNumber = DBNull.Value, SceneEditionNumber = DBNull.Value }).Where(new { SceneBookNumber = 0, SceneEditionNumber = 0 });
        }
    }
}
