using System;
using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.MetadataSource.BookInfo.Resources
{
    public class AuthorResource
    {
        public string ForeignAuthorId { get; set; }
        public string GoodreadsId { get; set; }
        public string IsniId { get; set; }
        public string AsinId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorNameLastFirst { get; set; }
        public string Overview { get; set; }
        public string Gender { get; set; }
        public DateTime? Born { get; set; }
        public DateTime? Died { get; set; }
        public string Website { get; set; }
        public string Status { get; set; }
        public List<ImageResource> Images { get; set; }
        public List<string> Genres { get; set; }
        public List<LinkResource> Links { get; set; }
        public List<string> Aliases { get; set; }
        public RatingsResource Ratings { get; set; }
        public List<BookResource> Books { get; set; }
        public List<SeriesResource> Series { get; set; }
    }
}