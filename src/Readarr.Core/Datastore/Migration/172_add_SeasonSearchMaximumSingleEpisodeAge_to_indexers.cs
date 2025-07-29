using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(172)]
    public class add_SeasonSearchMaximumSingleEpisodeAge_to_indexers : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Indexers").AddColumn("SeasonSearchMaximumSingleEpisodeAge").AsInt32().NotNullable().WithDefaultValue(0);
        }
    }
}
