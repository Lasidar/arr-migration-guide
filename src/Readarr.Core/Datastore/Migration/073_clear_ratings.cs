using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(73)]
    public class clear_ratings : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Update.Table("Series")
                  .Set(new { Ratings = "{}" })
                  .AllRows();

            Update.Table("Episodes")
                  .Set(new { Ratings = "{}" })
                  .AllRows();
        }
    }
}
