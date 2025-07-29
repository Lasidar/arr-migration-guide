using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(153)]
    public class add_on_episodefiledelete_for_upgrade : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Notifications").AddColumn("OnEpisodeFileDeleteForUpgrade").AsBoolean().WithDefaultValue(true);
        }
    }
}
