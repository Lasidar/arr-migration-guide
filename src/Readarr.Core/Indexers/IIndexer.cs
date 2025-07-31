using System.Collections.Generic;
using System.Threading.Tasks;
using Readarr.Common.Http;
using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.Parser.Model;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Tv;

namespace Readarr.Core.Indexers
{
    public interface IIndexer : IProvider
    {
        bool SupportsRss { get; }
        bool SupportsSearch { get; }
        DownloadProtocol Protocol { get; }

        Task<IList<ReleaseInfo>> FetchRecent();
        
        // Book search methods
        Task<IList<ReleaseInfo>> Fetch(BookSearchCriteria searchCriteria);
        Task<IList<ReleaseInfo>> Fetch(AuthorSearchCriteria searchCriteria);
        
        // TV search methods (to be removed)
        Task<IList<ReleaseInfo>> Fetch(SeasonSearchCriteria searchCriteria);
        Task<IList<ReleaseInfo>> Fetch(SingleEpisodeSearchCriteria searchCriteria);
        Task<IList<ReleaseInfo>> Fetch(DailyEpisodeSearchCriteria searchCriteria);
        Task<IList<ReleaseInfo>> Fetch(DailySeasonSearchCriteria searchCriteria);
        Task<IList<ReleaseInfo>> Fetch(AnimeEpisodeSearchCriteria searchCriteria);
        Task<IList<ReleaseInfo>> Fetch(AnimeSeasonSearchCriteria searchCriteria);
        Task<IList<ReleaseInfo>> Fetch(SpecialEpisodeSearchCriteria searchCriteria);
        
        HttpRequest GetDownloadRequest(string link);
    }
}
