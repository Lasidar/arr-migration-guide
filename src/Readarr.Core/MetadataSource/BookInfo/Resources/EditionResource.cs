using System;
using System.Collections.Generic;

namespace Readarr.Core.MetadataSource.BookInfo.Resources
{
    public class EditionResource
    {
        public string ForeignEditionId { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Asin { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public string Overview { get; set; }
        public string Format { get; set; }
        public bool IsEbook { get; set; }
        public string Publisher { get; set; }
        public int? PageCount { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public List<ImageResource> Images { get; set; }
        public RatingsResource Ratings { get; set; }
    }
}