using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(35)]
    public class add_series_folder_format_to_naming_config : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("NamingConfig").AddColumn("SeriesFolderFormat").AsString().Nullable();

            Update.Table("NamingConfig").Set(new { SeriesFolderFormat = "{Series Title}" }).AllRows();
        }
    }
}
