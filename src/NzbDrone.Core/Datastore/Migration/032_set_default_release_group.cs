using System;
using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(32)]
    public class set_default_release_group : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Update.Table("EditionFiles").Set(new { ReleaseGroup = "DRONE" }).Where(new { ReleaseGroup = DBNull.Value });
        }
    }
}
