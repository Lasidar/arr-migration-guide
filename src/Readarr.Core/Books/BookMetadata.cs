using System;
using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore;

namespace Readarr.Core.Books
{
    public class BookMetadata : ModelBase
    {
        public BookMetadata()
        {
            Images = new List<MediaCover.MediaCover>();
            Genres = new List<string>();
            Links = new List<Links>();
        }

        // Identifiers
        public string ForeignBookId { get; set; }
        public string GoodreadsId { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Asin { get; set; }
        
        // Basic Info
        public string Title { get; set; }
        public string SortTitle { get; set; }
        public string TitleSlug { get; set; }
        public string OriginalTitle { get; set; }
        public string Language { get; set; }
        
        // Book Details
        public string Overview { get; set; }
        public string Publisher { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? PageCount { get; set; }
        
        // Metadata
        public List<MediaCover.MediaCover> Images { get; set; }
        public List<string> Genres { get; set; }
        public List<Links> Links { get; set; }
        public Ratings Ratings { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}] {1}", ForeignBookId, Title.NullSafe());
        }
    }
}