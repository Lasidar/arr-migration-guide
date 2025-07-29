using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(120)]
    public class update_series_episodes_history_indexes : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.Index().OnTable("Episodes").OnColumn("SeriesId").Ascending()
                                              .OnColumn("AirDate").Ascending();

            Delete.Index().OnTable("History").OnColumn("EpisodeId");
            Create.Index().OnTable("History").OnColumn("EpisodeId").Ascending()
                                             .OnColumn("Date").Descending();

            Delete.Index().OnTable("History").OnColumn("DownloadId");
            Create.Index().OnTable("History").OnColumn("DownloadId").Ascending()
                                             .OnColumn("Date").Descending();
        }
    }
}
