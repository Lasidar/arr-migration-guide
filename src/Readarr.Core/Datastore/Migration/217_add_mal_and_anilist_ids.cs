using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(217)]
    public class add_mal_and_anilist_ids : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Series").AddColumn("MalIds").AsString().WithDefaultValue("[]");
            Alter.Table("Series").AddColumn("AniListIds").AsString().WithDefaultValue("[]");
        }
    }
}
