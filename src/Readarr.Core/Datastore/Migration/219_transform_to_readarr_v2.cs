using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(219)]
    public class transform_to_readarr_v2 : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            // Step 1: Create new metadata tables
            Create.TableForModel("AuthorMetadata")
                .WithColumn("ForeignAuthorId").AsString().NotNullable().Unique()
                .WithColumn("GoodreadsId").AsString().Nullable()
                .WithColumn("IsniId").AsString().Nullable()
                .WithColumn("AsinId").AsString().Nullable()
                .WithColumn("Name").AsString().NotNullable()
                .WithColumn("SortName").AsString().Nullable()
                .WithColumn("NameSlug").AsString().Nullable()
                .WithColumn("Overview").AsString().Nullable()
                .WithColumn("Gender").AsString().Nullable()
                .WithColumn("Hometown").AsString().Nullable()
                .WithColumn("Born").AsDateTime().Nullable()
                .WithColumn("Died").AsDateTime().Nullable()
                .WithColumn("Website").AsString().Nullable()
                .WithColumn("Status").AsInt32().NotNullable()
                .WithColumn("Images").AsString().NotNullable()
                .WithColumn("Genres").AsString().Nullable()
                .WithColumn("Links").AsString().Nullable()
                .WithColumn("Aliases").AsString().Nullable()
                .WithColumn("Ratings").AsString().Nullable();

            Create.TableForModel("BookMetadata")
                .WithColumn("ForeignBookId").AsString().NotNullable().Unique()
                .WithColumn("GoodreadsId").AsString().Nullable()
                .WithColumn("Isbn").AsString().Nullable()
                .WithColumn("Isbn13").AsString().Nullable()
                .WithColumn("Asin").AsString().Nullable()
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn("SortTitle").AsString().Nullable()
                .WithColumn("TitleSlug").AsString().Nullable()
                .WithColumn("OriginalTitle").AsString().Nullable()
                .WithColumn("Language").AsString().Nullable()
                .WithColumn("Overview").AsString().Nullable()
                .WithColumn("Publisher").AsString().Nullable()
                .WithColumn("ReleaseDate").AsDateTime().Nullable()
                .WithColumn("PageCount").AsInt32().Nullable()
                .WithColumn("Images").AsString().NotNullable()
                .WithColumn("Genres").AsString().Nullable()
                .WithColumn("Links").AsString().Nullable()
                .WithColumn("Ratings").AsString().Nullable();

            // Step 2: Rename Series table to Authors
            Rename.Table("Series").To("Authors");
            
            // Add new columns to Authors table
            Alter.Table("Authors")
                .AddColumn("AuthorMetadataId").AsInt32().NotNullable().WithDefaultValue(0)
                .AddColumn("CleanName").AsString().Nullable()
                .AddColumn("SortName").AsString().Nullable();

            // Rename columns in Authors table
            Rename.Column("Title").OnTable("Authors").To("Name");
            Rename.Column("CleanTitle").OnTable("Authors").To("CleanName");
            Rename.Column("TvdbId").OnTable("Authors").To("ForeignAuthorId");
            
            // Remove TV-specific columns
            Delete.Column("TvRageId").FromTable("Authors");
            Delete.Column("TvMazeId").FromTable("Authors");
            Delete.Column("ImdbId").FromTable("Authors");
            Delete.Column("TmdbId").FromTable("Authors");
            Delete.Column("Network").FromTable("Authors");
            Delete.Column("AirTime").FromTable("Authors");
            Delete.Column("SeriesType").FromTable("Authors");
            Delete.Column("UseSceneNumbering").FromTable("Authors");
            Delete.Column("FirstAired").FromTable("Authors");
            Delete.Column("LastAired").FromTable("Authors");
            Delete.Column("SeasonFolder").FromTable("Authors");

            // Step 3: Create Books table (was Seasons)
            Create.TableForModel("Books")
                .WithColumn("AuthorId").AsInt32().NotNullable()
                .WithColumn("BookMetadataId").AsInt32().NotNullable().WithDefaultValue(0)
                .WithColumn("ForeignBookId").AsString().NotNullable()
                .WithColumn("TitleSlug").AsString().Nullable()
                .WithColumn("Isbn").AsString().Nullable()
                .WithColumn("Isbn13").AsString().Nullable()
                .WithColumn("Asin").AsString().Nullable()
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn("CleanTitle").AsString().NotNullable()
                .WithColumn("SortTitle").AsString().Nullable()
                .WithColumn("Overview").AsString().Nullable()
                .WithColumn("Monitored").AsBoolean().NotNullable()
                .WithColumn("AnyEditionOk").AsBoolean().NotNullable()
                .WithColumn("Ratings").AsString().Nullable()
                .WithColumn("ReleaseDate").AsDateTime().Nullable()
                .WithColumn("PageCount").AsInt32().Nullable()
                .WithColumn("Genres").AsString().Nullable()
                .WithColumn("Images").AsString().NotNullable()
                .WithColumn("Links").AsString().Nullable()
                .WithColumn("SeriesId").AsInt32().Nullable()
                .WithColumn("SeriesPosition").AsString().Nullable()
                .WithColumn("Added").AsDateTime().NotNullable()
                .WithColumn("LastInfoSync").AsDateTime().Nullable()
                .WithColumn("AddOptions").AsString().Nullable();

            // Step 4: Transform Episodes to Editions
            Rename.Table("Episodes").To("Editions");
            
            // Add new columns to Editions
            Alter.Table("Editions")
                .AddColumn("BookId").AsInt32().NotNullable().WithDefaultValue(0)
                .AddColumn("ForeignEditionId").AsString().Nullable()
                .AddColumn("Isbn").AsString().Nullable()
                .AddColumn("Isbn13").AsString().Nullable()
                .AddColumn("Asin").AsString().Nullable()
                .AddColumn("Language").AsString().Nullable()
                .AddColumn("Format").AsString().Nullable()
                .AddColumn("IsEbook").AsBoolean().NotNullable().WithDefaultValue(false)
                .AddColumn("Publisher").AsString().Nullable()
                .AddColumn("PageCount").AsInt32().Nullable()
                .AddColumn("ManualAdd").AsBoolean().NotNullable().WithDefaultValue(false);

            // Rename columns in Editions
            Rename.Column("SeriesId").OnTable("Editions").To("AuthorId");
            Rename.Column("SeasonNumber").OnTable("Editions").To("BookNumber");
            Rename.Column("EpisodeNumber").OnTable("Editions").To("EditionNumber");
            Rename.Column("EpisodeFileId").OnTable("Editions").To("BookFileId");
            Rename.Column("AirDate").OnTable("Editions").To("ReleaseDate");
            Rename.Column("AirDateUtc").OnTable("Editions").To("ReleaseDateUtc");

            // Remove TV-specific columns from Editions
            Delete.Column("TvdbId").FromTable("Editions");
            Delete.Column("AbsoluteEpisodeNumber").FromTable("Editions");
            Delete.Column("SceneAbsoluteEpisodeNumber").FromTable("Editions");
            Delete.Column("SceneSeasonNumber").FromTable("Editions");
            Delete.Column("SceneEpisodeNumber").FromTable("Editions");
            Delete.Column("AiredAfterSeasonNumber").FromTable("Editions");
            Delete.Column("AiredBeforeSeasonNumber").FromTable("Editions");
            Delete.Column("AiredBeforeEpisodeNumber").FromTable("Editions");
            Delete.Column("UnverifiedSceneNumbering").FromTable("Editions");
            Delete.Column("Runtime").FromTable("Editions");
            Delete.Column("FinaleType").FromTable("Editions");

            // Step 5: Create Book Series table
            Create.TableForModel("Series")
                .WithColumn("ForeignSeriesId").AsString().NotNullable()
                .WithColumn("Title").AsString().NotNullable()
                .WithColumn("CleanTitle").AsString().NotNullable()
                .WithColumn("SortTitle").AsString().Nullable()
                .WithColumn("Description").AsString().Nullable()
                .WithColumn("Numbered").AsBoolean().NotNullable()
                .WithColumn("AuthorId").AsInt32().NotNullable();

            Create.TableForModel("SeriesBookLink")
                .WithColumn("SeriesId").AsInt32().NotNullable()
                .WithColumn("BookId").AsInt32().NotNullable()
                .WithColumn("Position").AsString().Nullable()
                .WithColumn("PositionOrder").AsInt32().Nullable();

            // Step 6: Transform EpisodeFiles to BookFiles
            Rename.Table("EpisodeFiles").To("BookFiles");
            
            // Add new columns to BookFiles
            Alter.Table("BookFiles")
                .AddColumn("AuthorId").AsInt32().NotNullable().WithDefaultValue(0)
                .AddColumn("BookId").AsInt32().NotNullable().WithDefaultValue(0)
                .AddColumn("EditionId").AsInt32().NotNullable().WithDefaultValue(0)
                .AddColumn("CalibreId").AsString().Nullable()
                .AddColumn("Part").AsInt32().NotNullable().WithDefaultValue(1);

            // Rename columns in BookFiles
            Rename.Column("SeriesId").OnTable("BookFiles").To("AuthorId");
            Rename.Column("SeasonNumber").OnTable("BookFiles").To("BookNumber");

            // Step 7: Update other tables
            // Update History table
            Alter.Table("History")
                .AddColumn("AuthorId").AsInt32().NotNullable().WithDefaultValue(0)
                .AddColumn("BookId").AsInt32().NotNullable().WithDefaultValue(0);
            
            Rename.Column("SeriesId").OnTable("History").To("AuthorId");
            Rename.Column("EpisodeId").OnTable("History").To("EditionId");

            // Update Blacklist table
            Rename.Column("SeriesId").OnTable("Blacklist").To("AuthorId");
            Rename.Column("EpisodeIds").OnTable("Blacklist").To("EditionIds");

            // Update ImportLists table
            Alter.Table("ImportLists")
                .AlterColumn("SeriesType").AsInt32().Nullable();

            // Update Notifications table - rename event types
            Execute.Sql(@"UPDATE ""Notifications"" 
                         SET ""OnSeriesAdd"" = ""OnAuthorAdd"",
                             ""OnSeriesDelete"" = ""OnAuthorDelete"",
                             ""OnEpisodeFileDelete"" = ""OnBookFileDelete"",
                             ""OnEpisodeFileDeleteForUpgrade"" = ""OnBookFileDeleteForUpgrade""
                         WHERE 1=1");

            // Step 8: Create indexes
            Create.Index("IX_Authors_CleanName").OnTable("Authors").OnColumn("CleanName");
            Create.Index("IX_Authors_AuthorMetadataId").OnTable("Authors").OnColumn("AuthorMetadataId");
            Create.Index("IX_Books_AuthorId").OnTable("Books").OnColumn("AuthorId");
            Create.Index("IX_Books_CleanTitle").OnTable("Books").OnColumn("CleanTitle");
            Create.Index("IX_Editions_BookId").OnTable("Editions").OnColumn("BookId");
            Create.Index("IX_BookFiles_AuthorId").OnTable("BookFiles").OnColumn("AuthorId");
            Create.Index("IX_SeriesBookLink_SeriesId").OnTable("SeriesBookLink").OnColumn("SeriesId");
            Create.Index("IX_SeriesBookLink_BookId").OnTable("SeriesBookLink").OnColumn("BookId");
        }
    }
}