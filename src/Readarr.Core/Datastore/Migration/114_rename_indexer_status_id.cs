using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(114)]
    public class rename_indexer_status_id : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Rename.Column("IndexerId").OnTable("IndexerStatus").To("ProviderId");
        }
    }
}
