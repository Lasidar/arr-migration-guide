using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(2)]
    public class remove_tvrage_imdb_unique_constraint : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Index().OnTable("Series").OnColumn("TvRageId");
            Delete.Index().OnTable("Series").OnColumn("ImdbId");
        }
    }
}
