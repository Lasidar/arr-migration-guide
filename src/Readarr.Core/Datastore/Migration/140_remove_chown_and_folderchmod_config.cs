using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(140)]
    public class remove_chown_and_folderchmod_config_v2 : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Execute.Sql("DELETE FROM \"Config\" WHERE \"Key\" IN ('folderchmod', 'chownuser')");

            // Note: v1 version of migration removed 'chowngroup'
        }
    }
}
