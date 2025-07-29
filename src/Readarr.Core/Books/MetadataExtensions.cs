namespace Readarr.Core.Books
{
    public static class MetadataExtensions
    {
        public static void ApplyChanges(this AuthorMetadata local, AuthorMetadata remote)
        {
            local.ForeignAuthorId = remote.ForeignAuthorId;
            local.GoodreadsId = remote.GoodreadsId;
            local.IsniId = remote.IsniId;
            local.AsinId = remote.AsinId;
            local.Name = remote.Name;
            local.Overview = remote.Overview;
            local.Gender = remote.Gender;
            local.Born = remote.Born;
            local.Died = remote.Died;
            local.Website = remote.Website;
            local.Status = remote.Status;
            local.Images = remote.Images;
            local.Genres = remote.Genres;
            local.Links = remote.Links;
            local.Aliases = remote.Aliases;
            local.Ratings = remote.Ratings;
            local.SortName = remote.SortName;
            local.NameLastFirst = remote.NameLastFirst;
        }

        public static void ApplyChanges(this BookMetadata local, BookMetadata remote)
        {
            local.ForeignBookId = remote.ForeignBookId;
            local.GoodreadsId = remote.GoodreadsId;
            local.Isbn = remote.Isbn;
            local.Isbn13 = remote.Isbn13;
            local.Asin = remote.Asin;
            local.Title = remote.Title;
            local.SortTitle = remote.SortTitle;
            local.TitleSlug = remote.TitleSlug;
            local.OriginalTitle = remote.OriginalTitle;
            local.Language = remote.Language;
            local.Overview = remote.Overview;
            local.Publisher = remote.Publisher;
            local.ReleaseDate = remote.ReleaseDate;
            local.PageCount = remote.PageCount;
            local.Images = remote.Images;
            local.Genres = remote.Genres;
            local.Links = remote.Links;
            local.Ratings = remote.Ratings;
        }
    }
}