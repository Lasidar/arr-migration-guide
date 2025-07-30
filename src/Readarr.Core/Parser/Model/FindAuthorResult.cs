using Readarr.Core.Books;

namespace Readarr.Core.Parser.Model
{
    public class FindAuthorResult
    {
        public Author Author { get; set; }
        public AuthorMatchType MatchType { get; set; }

        public FindAuthorResult(Author author, AuthorMatchType matchType)
        {
            Author = author;
            MatchType = matchType;
        }
    }

    public enum AuthorMatchType
    {
        Unknown = 0,
        Title = 1,
        Alias = 2,
        Id = 3
    }
}