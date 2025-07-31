using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(14)]
    public class drop_air_date : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("AirDate").FromTable("Episodes");
        }
    }
}
