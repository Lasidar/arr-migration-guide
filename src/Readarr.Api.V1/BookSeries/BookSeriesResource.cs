using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Extensions;
using Readarr.Core.Languages;
using Readarr.Core.MediaCover;
using Readarr.Core.Books;
using Readarr.Http.REST;
using Readarr.Api.V1.Book;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Api.V1.BookSeries
{
    public class BookSeriesResource : RestResource
    {
        // Basic Info
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Numbered { get; set; }
        
        // IDs
        public string ForeignSeriesId { get; set; }
        public int AuthorId { get; set; }
        
        // Related Data
        public List<SeriesBookResource> Books { get; set; }
    }

    public class SeriesBookResource : RestResource
    {
        public int BookId { get; set; }
        public string Position { get; set; }
        public BookResource Book { get; set; }
    }

    public static class BookSeriesResourceMapper
    {
        public static BookSeriesResource ToResource(this Core.Books.Series model)
        {
            if (model == null)
            {
                return null;
            }

            return new BookSeriesResource
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Numbered = model.Numbered,
                ForeignSeriesId = model.ForeignSeriesId,
                AuthorId = model.AuthorId
            };
        }

        public static Core.Books.Series ToModel(this BookSeriesResource resource)
        {
            if (resource == null)
            {
                return null;
            }

            return new Core.Books.Series
            {
                Id = resource.Id,
                Title = resource.Title,
                Description = resource.Description,
                Numbered = resource.Numbered,
                ForeignSeriesId = resource.ForeignSeriesId,
                AuthorId = resource.AuthorId
            };
        }

        public static List<BookSeriesResource> ToResource(this IEnumerable<Core.Books.Series> models)
        {
            return models?.Select(ToResource).ToList();
        }

        public static SeriesBookResource ToResource(this SeriesBookLink model)
        {
            if (model == null)
            {
                return null;
            }

            return new SeriesBookResource
            {
                Id = model.Id,
                BookId = model.BookId,
                Position = model.Position
            };
        }

        public static List<SeriesBookResource> ToResource(this IEnumerable<SeriesBookLink> models)
        {
            return models?.Select(ToResource).ToList();
        }
    }
}
