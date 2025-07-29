using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Api.V1.Author;
using Readarr.Api.V1.Edition;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Book
{
    public class BookResource : RestResource
    {
        // Basic Info
        public string Title { get; set; }
        public string SortTitle { get; set; }
        public string Overview { get; set; }
        
        // IDs
        public int AuthorId { get; set; }
        public string ForeignBookId { get; set; }
        public string TitleSlug { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Asin { get; set; }
        
        // Monitoring
        public bool Monitored { get; set; }
        public bool AnyEditionOk { get; set; }
        
        // Metadata
        public Ratings Ratings { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int PageCount { get; set; }
        public List<string> Genres { get; set; }
        public List<MediaCover> Images { get; set; }
        public List<Links> Links { get; set; }
        
        // Series Info
        public int? SeriesId { get; set; }
        public string SeriesPosition { get; set; }
        
        // System
        public DateTime Added { get; set; }
        public DateTime? LastInfoSync { get; set; }
        public AddBookOptions AddOptions { get; set; }
        
        // Related Data
        public AuthorResource Author { get; set; }
        public List<EditionResource> Editions { get; set; }
        public BookMetadataResource Metadata { get; set; }
        public BookStatisticsResource Statistics { get; set; }
    }

    public class BookMetadataResource : RestResource
    {
        public string ForeignBookId { get; set; }
        public string GoodreadsId { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Asin { get; set; }
        public string Title { get; set; }
        public string SortTitle { get; set; }
        public string TitleSlug { get; set; }
        public string OriginalTitle { get; set; }
        public string Language { get; set; }
        public string Overview { get; set; }
        public string Publisher { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? PageCount { get; set; }
        public List<MediaCover> Images { get; set; }
        public List<string> Genres { get; set; }
        public List<Links> Links { get; set; }
        public Ratings Ratings { get; set; }
    }

    public class BookStatisticsResource
    {
        public int BookFileCount { get; set; }
        public int EditionCount { get; set; }
        public int TotalEditionCount { get; set; }
        public long SizeOnDisk { get; set; }
        public decimal PercentOfEditions
        {
            get
            {
                if (EditionCount == 0)
                {
                    return 0;
                }

                return TotalEditionCount > 0 ? EditionCount / (decimal)TotalEditionCount * 100 : 0;
            }
        }
    }

    public static class BookResourceMapper
    {
        public static BookResource ToResource(this Core.Books.Book model)
        {
            if (model == null)
            {
                return null;
            }

            return new BookResource
            {
                Id = model.Id,
                AuthorId = model.AuthorId,
                
                // Basic info
                Title = model.Metadata.Value?.Title ?? "Unknown",
                SortTitle = model.SortTitle,
                Overview = model.Metadata.Value?.Overview,
                
                // IDs
                ForeignBookId = model.Metadata.Value?.ForeignBookId,
                TitleSlug = model.Metadata.Value?.TitleSlug,
                Isbn = model.Metadata.Value?.Isbn,
                Isbn13 = model.Metadata.Value?.Isbn13,
                Asin = model.Metadata.Value?.Asin,
                
                // Monitoring
                Monitored = model.Monitored,
                AnyEditionOk = model.AnyEditionOk,
                
                // Metadata
                Ratings = model.Metadata.Value?.Ratings,
                ReleaseDate = model.Metadata.Value?.ReleaseDate,
                PageCount = model.Metadata.Value?.PageCount ?? 0,
                Genres = model.Metadata.Value?.Genres ?? new List<string>(),
                Images = model.Metadata.Value?.Images ?? new List<MediaCover>(),
                Links = model.Metadata.Value?.Links ?? new List<Links>(),
                
                // Series
                SeriesId = model.SeriesId,
                SeriesPosition = model.SeriesPosition,
                
                // System
                Added = model.Added,
                LastInfoSync = model.LastInfoSync,
                AddOptions = model.AddOptions,
                
                // Metadata object
                Metadata = model.Metadata.Value?.ToResource()
            };
        }

        public static Core.Books.Book ToModel(this BookResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new Core.Books.Book
            {
                Id = resource.Id,
                AuthorId = resource.AuthorId,
                
                // Names
                CleanTitle = resource.Title.CleanSeriesTitle(),
                SortTitle = resource.SortTitle,
                
                // Monitoring
                Monitored = resource.Monitored,
                AnyEditionOk = resource.AnyEditionOk,
                
                // Series
                SeriesId = resource.SeriesId,
                SeriesPosition = resource.SeriesPosition,
                
                // System
                Added = resource.Added,
                LastInfoSync = resource.LastInfoSync,
                AddOptions = resource.AddOptions
            };
        }

        public static BookResource ToResource(this Core.Books.Book model, bool includeAuthor)
        {
            var resource = model.ToResource();
            
            if (includeAuthor && model.Author != null && model.Author.IsLoaded)
            {
                resource.Author = model.Author.Value.ToResource();
            }

            return resource;
        }

        public static List<BookResource> ToResource(this IEnumerable<Core.Books.Book> models)
        {
            return models?.Select(ToResource).ToList();
        }

        public static BookMetadataResource ToResource(this BookMetadata model)
        {
            if (model == null)
            {
                return null;
            }

            return new BookMetadataResource
            {
                Id = model.Id,
                ForeignBookId = model.ForeignBookId,
                GoodreadsId = model.GoodreadsId,
                Isbn = model.Isbn,
                Isbn13 = model.Isbn13,
                Asin = model.Asin,
                Title = model.Title,
                SortTitle = model.SortTitle,
                TitleSlug = model.TitleSlug,
                OriginalTitle = model.OriginalTitle,
                Language = model.Language,
                Overview = model.Overview,
                Publisher = model.Publisher,
                ReleaseDate = model.ReleaseDate,
                PageCount = model.PageCount,
                Images = model.Images,
                Genres = model.Genres,
                Links = model.Links,
                Ratings = model.Ratings
            };
        }
    }
}