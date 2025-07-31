using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(12)]
    public class remove_custom_start_date : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("CustomStartDate").FromTable("Series");
        }
    }
}
