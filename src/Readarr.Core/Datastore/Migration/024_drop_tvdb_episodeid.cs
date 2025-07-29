using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(24)]
    public class drop_tvdb_episodeid : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("TvDbEpisodeId").FromTable("Episodes");
        }
    }
}
