using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(21)]
    public class drop_seasons_table : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Table("Seasons");
        }
    }
}
