using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(124)]
    public class remove_media_browser_metadata : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.FromTable("Metadata").Row(new { Implementation = "MediaBrowserMetadata" });
            Delete.FromTable("MetadataFiles").Row(new { Consumer = "MediaBrowserMetadata" });
        }
    }
}
