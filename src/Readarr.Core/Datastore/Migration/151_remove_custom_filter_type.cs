using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(151)]
    public class remove_custom_filter_type : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Update.Table("CustomFilters").Set(new { Type = "series" }).Where(new { Type = "seriesIndex" });
            Update.Table("CustomFilters").Set(new { Type = "series" }).Where(new { Type = "seriesEditor" });
            Update.Table("CustomFilters").Set(new { Type = "series" }).Where(new { Type = "seasonPass" });
        }
    }
}
