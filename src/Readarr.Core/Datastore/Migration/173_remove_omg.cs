using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(173)]
    public class remove_omg : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Execute.Sql("DELETE FROM \"Indexers\" WHERE \"Implementation\" = 'Omgwtfnzbs'");
        }
    }
}
