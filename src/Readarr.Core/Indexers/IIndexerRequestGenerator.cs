using Readarr.Core.IndexerSearch.Definitions;

namespace Readarr.Core.Indexers
{
    public interface IIndexerRequestGenerator
    {
        IndexerPageableRequestChain GetRecentRequests();
        
        // Book search methods
        IndexerPageableRequestChain GetSearchRequests(BookSearchCriteria searchCriteria);
        IndexerPageableRequestChain GetSearchRequests(AuthorSearchCriteria searchCriteria);
        
        // TV search methods (to be removed)
        IndexerPageableRequestChain GetSearchRequests(SingleEpisodeSearchCriteria searchCriteria);
        IndexerPageableRequestChain GetSearchRequests(SeasonSearchCriteria searchCriteria);
        IndexerPageableRequestChain GetSearchRequests(DailyEpisodeSearchCriteria searchCriteria);
        IndexerPageableRequestChain GetSearchRequests(DailySeasonSearchCriteria searchCriteria);
        IndexerPageableRequestChain GetSearchRequests(AnimeEpisodeSearchCriteria searchCriteria);
        IndexerPageableRequestChain GetSearchRequests(AnimeSeasonSearchCriteria searchCriteria);
        IndexerPageableRequestChain GetSearchRequests(SpecialEpisodeSearchCriteria searchCriteria);
    }
}
