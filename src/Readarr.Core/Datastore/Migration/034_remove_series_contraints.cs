using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(34)]
    public class remove_series_contraints : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Series")
                .AlterColumn("ImdbId").AsString().Nullable()
                .AlterColumn("TitleSlug").AsString().Nullable();
        }
    }
}
