using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(206)]
    public class add_tmdbid : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Series").AddColumn("TmdbId").AsInt32().WithDefaultValue(0);
            Create.Index().OnTable("Series").OnColumn("TmdbId");
        }
    }
}
