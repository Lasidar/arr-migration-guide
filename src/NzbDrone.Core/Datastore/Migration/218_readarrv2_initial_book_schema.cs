using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(218)]
    public class readarrv2_initial_book_schema : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            // Rename Series table to Authors
            Rename.Table("Series").To("Authors");
            Rename.Column("TvdbId").OnTable("Authors").To("GoodreadsId");
            Rename.Column("SeriesType").OnTable("Authors").To("AuthorType");
            Rename.Column("Title").OnTable("Authors").To("Name");
            Rename.Column("CleanTitle").OnTable("Authors").To("CleanName");
            Rename.Column("SortTitle").OnTable("Authors").To("SortName");
            Rename.Column("TitleSlug").OnTable("Authors").To("NameSlug");
            Rename.Column("Overview").OnTable("Authors").To("Biography");
            Rename.Column("AirTime").OnTable("Authors").To("BirthDate");
            Rename.Column("Network").OnTable("Authors").To("Publisher");
            Rename.Column("FirstAired").OnTable("Authors").To("FirstPublished");
            Rename.Column("SeasonFolder").OnTable("Authors").To("BookFolder");

            // Add new columns for Authors
            Alter.Table("Authors")
                .AddColumn("AmazonId").AsString().Nullable()
                .AddColumn("WikipediaId").AsString().Nullable()
                .AddColumn("IsbnId").AsString().Nullable()
                .AddColumn("DeathDate").AsDateTime().Nullable();

            // Rename Seasons table to Books
            Rename.Table("Seasons").To("Books");
            Rename.Column("SeriesId").OnTable("Books").To("AuthorId");
            Rename.Column("SeasonNumber").OnTable("Books").To("BookNumber");

            // Add new columns for Books
            Alter.Table("Books")
                .AddColumn("Title").AsString().Nullable()
                .AddColumn("Isbn").AsString().Nullable()
                .AddColumn("Isbn13").AsString().Nullable()
                .AddColumn("PageCount").AsInt32().WithDefaultValue(0)
                .AddColumn("PublishDate").AsDateTime().Nullable()
                .AddColumn("Publisher").AsString().Nullable()
                .AddColumn("Language").AsString().Nullable()
                .AddColumn("Overview").AsString().Nullable();

            // Rename Episodes table to Editions
            Rename.Table("Episodes").To("Editions");
            Rename.Column("SeriesId").OnTable("Editions").To("AuthorId");
            Rename.Column("TvdbId").OnTable("Editions").To("GoodreadsEditionId");
            Rename.Column("EpisodeFileId").OnTable("Editions").To("EditionFileId");
            Rename.Column("SeasonNumber").OnTable("Editions").To("BookNumber");
            Rename.Column("EpisodeNumber").OnTable("Editions").To("EditionNumber");
            Rename.Column("AirDate").OnTable("Editions").To("PublishDate");
            Rename.Column("AirDateUtc").OnTable("Editions").To("PublishDateUtc");
            Rename.Column("AbsoluteEpisodeNumber").OnTable("Editions").To("AbsoluteEditionNumber");
            Rename.Column("SceneAbsoluteEpisodeNumber").OnTable("Editions").To("SceneAbsoluteEditionNumber");
            Rename.Column("SceneSeasonNumber").OnTable("Editions").To("SceneBookNumber");
            Rename.Column("SceneEpisodeNumber").OnTable("Editions").To("SceneEditionNumber");

            // Add new columns for Editions
            Alter.Table("Editions")
                .AddColumn("BookId").AsInt32().WithDefaultValue(0)
                .AddColumn("Isbn").AsString().Nullable()
                .AddColumn("Isbn13").AsString().Nullable()
                .AddColumn("Format").AsString().Nullable()
                .AddColumn("PageCount").AsInt32().WithDefaultValue(0)
                .AddColumn("Publisher").AsString().Nullable()
                .AddColumn("Language").AsString().Nullable();

            // Rename EpisodeFiles table to EditionFiles
            Rename.Table("EpisodeFiles").To("EditionFiles");
            Rename.Column("SeriesId").OnTable("EditionFiles").To("AuthorId");
            Rename.Column("SeasonNumber").OnTable("EditionFiles").To("BookNumber");

            // Update references in other tables
            Rename.Column("SeriesId").OnTable("History").To("AuthorId");
            Rename.Column("EpisodeId").OnTable("History").To("EditionId");

            Rename.Column("SeriesId").OnTable("ImportLists").To("AuthorId");
            
            Rename.Column("SeriesId").OnTable("Blocklist").To("AuthorId");
            
            Rename.Column("SeriesId").OnTable("PendingReleases").To("AuthorId");

            // Update SeriesStatistics to AuthorStatistics
            Rename.Table("SeriesStatistics").To("AuthorStatistics");
            Rename.Column("SeriesId").OnTable("AuthorStatistics").To("AuthorId");
            Rename.Column("SeasonCount").OnTable("AuthorStatistics").To("BookCount");
            Rename.Column("EpisodeFileCount").OnTable("AuthorStatistics").To("EditionFileCount");
            Rename.Column("EpisodeCount").OnTable("AuthorStatistics").To("EditionCount");
            Rename.Column("TotalEpisodeCount").OnTable("AuthorStatistics").To("TotalEditionCount");
        }
    }
}