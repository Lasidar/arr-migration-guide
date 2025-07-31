using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Search
{
    public class AuthorSearchResource : RestResource
    {
        public string ForeignAuthorId { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public List<MediaCover> Images { get; set; }
        public AuthorStatusType Status { get; set; }
        public Ratings Ratings { get; set; }
        public List<string> Genres { get; set; }
    }
}