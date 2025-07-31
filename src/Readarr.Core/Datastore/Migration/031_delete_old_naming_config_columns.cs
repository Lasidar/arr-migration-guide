using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(31)]
    public class delete_old_naming_config_columns : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("Separator")
                  .Column("NumberStyle")
                  .Column("IncludeSeriesTitle")
                  .Column("IncludeEpisodeTitle")
                  .Column("IncludeQuality")
                  .Column("ReplaceSpaces")
                  .FromTable("NamingConfig");
        }
    }
}
