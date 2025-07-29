using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(148)]
    public class mediainfo_channels : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Execute.Sql("UPDATE \"EpisodeFiles\" SET \"MediaInfo\" = Replace(\"MediaInfo\", '\"audioChannels\"', '\"audioChannelsContainer\"');");
            Execute.Sql("UPDATE \"EpisodeFiles\" SET \"MediaInfo\" = Replace(\"MediaInfo\", '\"audioChannelPositionsText\"', '\"audioChannelPositionsTextContainer\"');");
        }
    }
}
