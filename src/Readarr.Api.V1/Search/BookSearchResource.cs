using System;
using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Search
{
    public class BookSearchResource : RestResource
    {
        public string ForeignBookId { get; set; }
        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Overview { get; set; }
        public int PageCount { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public string AuthorName { get; set; }
        public List<MediaCover> Images { get; set; }
        public Ratings Ratings { get; set; }
        public List<string> Genres { get; set; }
        public bool IsExisting { get; set; }
    }
}