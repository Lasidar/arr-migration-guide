using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(60)]
    public class remove_enable_from_indexers : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("Enable").FromTable("Indexers");
            Delete.Column("Protocol").FromTable("DownloadClients");
        }
    }
}
