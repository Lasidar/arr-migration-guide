using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.Parser
{
    public interface IParsingService
    {
        Author GetAuthor(string title);
        Author GetAuthorFromTag(string tag);
        RemoteBook Map(BookInfo parsedBookInfo, SearchCriteriaBase searchCriteria = null);
        RemoteBook Map(BookInfo parsedBookInfo, int authorId, IEnumerable<int> bookIds);
        Book GetBook(Author author, string bookTitle);
    }
}