using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(74)]
    public class disable_eztv : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Execute.Sql("UPDATE \"Indexers\" SET \"EnableRss\" = false, \"EnableSearch\" = false WHERE \"Implementation\" = 'Eztv' AND \"Settings\" LIKE '%ezrss.it%'");
        }
    }
}
