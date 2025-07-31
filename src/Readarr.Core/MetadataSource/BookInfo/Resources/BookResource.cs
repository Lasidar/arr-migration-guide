using System;
using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.MetadataSource.BookInfo.Resources
{
    public class BookResource
    {
        public string ForeignBookId { get; set; }
        public string GoodreadsId { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Asin { get; set; }
        public string Title { get; set; }
        public string TitleSlug { get; set; }
        public string OriginalTitle { get; set; }
        public string Language { get; set; }
        public string Overview { get; set; }
        public string Publisher { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? PageCount { get; set; }
        public List<ImageResource> Images { get; set; }
        public List<string> Genres { get; set; }
        public List<LinkResource> Links { get; set; }
        public RatingsResource Ratings { get; set; }
        public List<EditionResource> Editions { get; set; }
        public List<SeriesLinkResource> SeriesLinks { get; set; }
    }
}