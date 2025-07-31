using System;

namespace Readarr.Core.ImportLists.Custom
{
    public class CustomBookAPIResource
    {
        public string Title { get; set; }
        public string AuthorName { get; set; }
        public string Isbn { get; set; }
        public string Asin { get; set; }
        public string GoodreadsId { get; set; }
        public int Year { get; set; }
        public string Publisher { get; set; }
        public DateTime? ReleaseDate { get; set; }
    }
}