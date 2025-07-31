using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(220)]
    public class remove_tv_tables : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            // Remove TV-specific tables
            Delete.Table("Episodes").IfExists();
            Delete.Table("EpisodeFiles").IfExists();
            Delete.Table("Series").IfExists();
            Delete.Table("Seasons").IfExists();
            
            // Remove TV-specific columns from shared tables if they exist
            if (Schema.Table("History").Column("EpisodeId").Exists())
            {
                Delete.Column("EpisodeId").FromTable("History");
            }
            
            if (Schema.Table("Blocklist").Column("EpisodeIds").Exists())
            {
                Delete.Column("EpisodeIds").FromTable("Blocklist");
            }
            
            if (Schema.Table("PendingReleases").Column("ParsedEpisodeInfo").Exists())
            {
                Delete.Column("ParsedEpisodeInfo").FromTable("PendingReleases");
            }
            
            // Remove TV-specific indexes
            Delete.Index("IX_Episodes_SeriesId").OnTable("Episodes").IfExists();
            Delete.Index("IX_Episodes_AirDate").OnTable("Episodes").IfExists();
            Delete.Index("IX_EpisodeFiles_SeriesId").OnTable("EpisodeFiles").IfExists();
            Delete.Index("IX_Series_CleanTitle").OnTable("Series").IfExists();
            Delete.Index("IX_Series_ImdbId").OnTable("Series").IfExists();
            Delete.Index("IX_Series_TvdbId").OnTable("Series").IfExists();
        }
    }
}