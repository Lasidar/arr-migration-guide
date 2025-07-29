using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(184)]
    public class remove_invalid_roksbox_metadata_images : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            IfDatabase("sqlite").Execute.Sql("DELETE FROM \"MetadataFiles\" WHERE \"Consumer\" = 'RoksboxMetadata' AND \"Type\" = 5 AND (\"RelativePath\" LIKE '%/metadata/%' OR \"RelativePath\" LIKE '%\\metadata\\%')");
            IfDatabase("postgresql").Execute.Sql("DELETE FROM \"MetadataFiles\" WHERE \"Consumer\" = 'RoksboxMetadata' AND \"Type\" = 5 AND (\"RelativePath\" LIKE '%/metadata/%' OR \"RelativePath\" LIKE '%\\\\metadata\\\\%')");
        }
    }
}
