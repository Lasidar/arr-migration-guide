using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(202)]
    public class add_indexer_flags : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Blocklist").AddColumn("IndexerFlags").AsInt32().WithDefaultValue(0);
            Alter.Table("EpisodeFiles").AddColumn("IndexerFlags").AsInt32().WithDefaultValue(0);
        }
    }
}
