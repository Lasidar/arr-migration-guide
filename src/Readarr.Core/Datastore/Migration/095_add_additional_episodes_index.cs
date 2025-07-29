using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(95)]
    public class add_additional_episodes_index : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.Index().OnTable("Episodes").OnColumn("SeriesId").Ascending()
                                              .OnColumn("SeasonNumber").Ascending()
                                              .OnColumn("EpisodeNumber").Ascending();
        }
    }
}
