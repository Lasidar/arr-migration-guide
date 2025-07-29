using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(134)]
    public class add_specials_folder_format : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("NamingConfig").AddColumn("SpecialsFolderFormat").AsString().Nullable();

            Update.Table("NamingConfig").Set(new { SpecialsFolderFormat = "Specials" }).AllRows();
        }
    }
}
