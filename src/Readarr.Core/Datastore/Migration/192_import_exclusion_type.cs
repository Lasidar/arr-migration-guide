using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(192)]
    public class import_exclusion_type : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            IfDatabase("sqlite").Alter.Table("ImportListExclusions").AlterColumn("TvdbId").AsInt32();

            // PG cannot autocast varchar to integer
            IfDatabase("postgresql").Execute.Sql("ALTER TABLE \"ImportListExclusions\" ALTER COLUMN \"TvdbId\" TYPE INTEGER USING \"TvdbId\"::integer");
        }
    }
}
