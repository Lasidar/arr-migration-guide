using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(13)]
    public class add_air_date_utc : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Episodes").AddColumn("AirDateUtc").AsDateTime().Nullable();

            Execute.Sql("UPDATE \"Episodes\" SET \"AirDateUtc\" = \"AirDate\"");
        }
    }
}
