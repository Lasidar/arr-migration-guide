using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Books;

namespace Readarr.Core.IndexerSearch.Definitions
{
    public class BookSearchCriteria : SearchCriteriaBase
    {
        public string BookTitle { get; set; }
        public string AuthorName { get; set; }
        public string BookQuery { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public int? BookYear { get; set; }
        
        public Author Author { get; set; }
        public List<Book> Books { get; set; }

        public BookSearchCriteria()
        {
            Books = new List<Book>();
        }

        public override string ToString()
        {
            var parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(BookTitle))
            {
                parts.Add(BookTitle);
            }

            if (!string.IsNullOrWhiteSpace(AuthorName))
            {
                parts.Add($"by {AuthorName}");
            }

            if (!string.IsNullOrWhiteSpace(Isbn))
            {
                parts.Add($"ISBN: {Isbn}");
            }

            if (BookYear.HasValue)
            {
                parts.Add($"({BookYear})");
            }

            return string.Join(" ", parts);
        }
    }
}