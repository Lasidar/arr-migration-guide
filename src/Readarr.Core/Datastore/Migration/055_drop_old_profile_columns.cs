using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(55)]
    public class drop_old_profile_columns : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("QualityProfileId").FromTable("Series");
        }
    }
}
