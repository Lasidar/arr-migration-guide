using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(144)]
    public class import_lists_series_type_and_season_folder : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("ImportLists").AddColumn("SeriesType").AsInt32().WithDefaultValue(0);
            Alter.Table("ImportLists").AddColumn("SeasonFolder").AsBoolean().WithDefaultValue(true);
        }
    }
}
