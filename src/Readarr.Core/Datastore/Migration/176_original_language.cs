using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Languages;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(176)]
    public class original_language : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Series")
                .AddColumn("OriginalLanguage").AsInt32().WithDefaultValue((int)Language.English);
        }
    }
}
