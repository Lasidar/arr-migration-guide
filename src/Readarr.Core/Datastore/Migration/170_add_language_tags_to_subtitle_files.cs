using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(170)]
    public class add_language_tags_to_subtitle_files : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("SubtitleFiles").AddColumn("LanguageTags").AsString().Nullable();
        }
    }
}
