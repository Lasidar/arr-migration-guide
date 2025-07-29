using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.Parser
{
    public interface IParsingService
    {
        // Book methods
        Author GetAuthor(string title);
        Author GetAuthorFromTag(string tag);
        RemoteBook Map(BookInfo parsedBookInfo, SearchCriteriaBase searchCriteria = null);
        RemoteBook Map(BookInfo parsedBookInfo, int authorId, IEnumerable<int> bookIds);
        Book GetBook(Author author, string bookTitle);
        
        // TV methods (for compatibility)
        Tv.Series GetSeries(string title);
        ParsedEpisodeInfo ParseSpecialEpisodeTitle(ParsedEpisodeInfo parsedEpisodeInfo, string releaseTitle, int tvdbId, int tvRageId, SearchCriteriaBase searchCriteria = null);
        List<Episode> GetEpisodes(ParsedEpisodeInfo parsedEpisodeInfo, Tv.Series series, bool sceneSource, SearchCriteriaBase searchCriteria = null);
    }
}