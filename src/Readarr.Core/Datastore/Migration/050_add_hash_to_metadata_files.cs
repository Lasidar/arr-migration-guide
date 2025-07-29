using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(50)]
    public class add_hash_to_metadata_files : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("MetadataFiles").AddColumn("Hash").AsString().Nullable();
        }
    }
}
