using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(152)]
    public class update_btn_url_to_https : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Execute.Sql("UPDATE \"Indexers\" SET \"Settings\" = Replace(\"Settings\", 'http://api.broadcasthe.net', 'https://api.broadcasthe.net') WHERE \"Implementation\" = 'BroadcastheNet';");
        }
    }
}
