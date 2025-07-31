using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Books
{
    public class BookResource : RestResource
    {
        public string Title { get; set; }
        public string CleanTitle { get; set; }
        public string Overview { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorPath { get; set; }
        public string GoodreadsId { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int PageCount { get; set; }
        public List<string> Genres { get; set; }
        public bool Monitored { get; set; }
        public int QualityProfileId { get; set; }
        public List<MediaCover> Images { get; set; }
        public DateTime Added { get; set; }
        public BookStatisticsResource Statistics { get; set; }
        public Ratings Ratings { get; set; }

        // Hiding this so people don't think its usable (only used to set the initial state)
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool AddOptions { get; set; }
    }

    public static class BookResourceMapper
    {
        public static BookResource ToResource(this Book model)
        {
            if (model == null) return null;

            return new BookResource
            {
                Id = model.Id,
                Title = model.Title,
                CleanTitle = model.CleanTitle,
                Overview = model.Overview,
                AuthorId = model.AuthorId,
                AuthorName = model.Author?.Value?.Name,
                AuthorPath = model.Author?.Value?.Path,
                GoodreadsId = model.Metadata.Value?.GoodreadsId,
                Isbn = model.Metadata.Value?.Isbn13,
                Asin = model.Metadata.Value?.Asin,
                ReleaseDate = model.ReleaseDate,
                PageCount = model.PageCount,
                Genres = model.Genres,
                Monitored = model.Monitored,
                QualityProfileId = model.Author?.Value?.QualityProfileId ?? 0,
                Images = model.Metadata.Value?.Images ?? new List<MediaCover>(),
                Added = model.Added,
                Ratings = model.Ratings
            };
        }

        public static Book ToModel(this BookResource resource)
        {
            if (resource == null) return null;

            return new Book
            {
                Id = resource.Id,
                Title = resource.Title,
                CleanTitle = resource.CleanTitle,
                Overview = resource.Overview,
                AuthorId = resource.AuthorId,
                ReleaseDate = resource.ReleaseDate,
                PageCount = resource.PageCount,
                Genres = resource.Genres,
                Monitored = resource.Monitored,
                Added = resource.Added,
                Ratings = resource.Ratings
            };
        }

        public static Book ToModel(this BookResource resource, Book book)
        {
            var updatedBook = resource.ToModel();

            book.Monitored = updatedBook.Monitored;
            book.Overview = updatedBook.Overview;
            book.Ratings = updatedBook.Ratings;

            return book;
        }

        public static List<BookResource> ToResource(this IEnumerable<Book> models)
        {
            return models?.Select(ToResource).ToList();
        }
    }
}