using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Common.Http;
using Readarr.Common.Instrumentation;
using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.ThingiProvider;
using Readarr.Core.Parser;
using Readarr.Core.Tv;

namespace Readarr.Core.Indexers.Newznab
{
    public class NewznabBookRequestGenerator : IIndexerRequestGenerator
    {
        private readonly Logger _logger;
        private readonly INewznabCapabilitiesProvider _capabilitiesProvider;

        public ProviderDefinition Definition { get; set; }
        public int MaxPages { get; set; }
        public int PageSize { get; set; }
        public NewznabSettings Settings { get; set; }

        public NewznabBookRequestGenerator(INewznabCapabilitiesProvider capabilitiesProvider)
        {
            _logger = ReadarrLogger.GetLogger(GetType());
            _capabilitiesProvider = capabilitiesProvider;

            MaxPages = 30;
            PageSize = 100;
        }

        private bool SupportsBookSearch
        {
            get
            {
                var capabilities = _capabilitiesProvider.GetCapabilities(Settings);

                return capabilities.SupportedSearchParameters != null &&
                       capabilities.SupportedSearchParameters.Contains("q");
            }
        }

        private bool SupportsAuthorSearch
        {
            get
            {
                var capabilities = _capabilitiesProvider.GetCapabilities(Settings);

                return capabilities.SupportedSearchParameters != null &&
                       capabilities.SupportedSearchParameters.Contains("author");
            }
        }

        private bool SupportsIsbnSearch
        {
            get
            {
                var capabilities = _capabilitiesProvider.GetCapabilities(Settings);

                return capabilities.SupportedSearchParameters != null &&
                       capabilities.SupportedSearchParameters.Contains("isbn");
            }
        }

        public virtual IndexerPageableRequestChain GetRecentRequests()
        {
            var pageableRequests = new IndexerPageableRequestChain();

            var capabilities = _capabilitiesProvider.GetCapabilities(Settings);

            if (capabilities.SupportedBookSearchParameters != null)
            {
                pageableRequests.Add(GetPagedRequests(MaxPages, Settings.Categories, "search", ""));
            }

            return pageableRequests;
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(AuthorSearchCriteria searchCriteria)
        {
            var pageableRequests = new IndexerPageableRequestChain();

            if (SupportsAuthorSearch)
            {
                AddAuthorPageableRequests(pageableRequests, searchCriteria);
            }
            else if (SupportsBookSearch)
            {
                pageableRequests.Add(GetPagedRequests(MaxPages, Settings.Categories, "search",
                    NewHttpUri().SetQueryParam("q", searchCriteria.AuthorName.CleanAuthorName()).Query));
            }

            return pageableRequests;
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(BookSearchCriteria searchCriteria)
        {
            var pageableRequests = new IndexerPageableRequestChain();

            if (searchCriteria.Isbn.IsNotNullOrWhiteSpace() && SupportsIsbnSearch)
            {
                pageableRequests.Add(GetPagedRequests(MaxPages, Settings.Categories, "search",
                    NewHttpUri().SetQueryParam("isbn", searchCriteria.Isbn.Replace("-", "")).Query));
            }
            else if (SupportsBookSearch)
            {
                var query = string.Empty;
                
                if (searchCriteria.BookTitle.IsNotNullOrWhiteSpace())
                {
                    query = searchCriteria.BookTitle;
                }
                
                if (searchCriteria.AuthorName.IsNotNullOrWhiteSpace())
                {
                    query = query.IsNotNullOrWhiteSpace() 
                        ? $"{query} {searchCriteria.AuthorName}"
                        : searchCriteria.AuthorName;
                }

                if (query.IsNotNullOrWhiteSpace())
                {
                    pageableRequests.Add(GetPagedRequests(MaxPages, Settings.Categories, "search",
                        NewHttpUri().SetQueryParam("q", query.CleanBookTitle()).Query));
                }
            }

            return pageableRequests;
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(IsbnSearchCriteria searchCriteria)
        {
            var pageableRequests = new IndexerPageableRequestChain();

            if (SupportsIsbnSearch)
            {
                pageableRequests.Add(GetPagedRequests(MaxPages, Settings.Categories, "search",
                    NewHttpUri().SetQueryParam("isbn", searchCriteria.CleanIsbn).Query));
            }
            else if (SupportsBookSearch)
            {
                pageableRequests.Add(GetPagedRequests(MaxPages, Settings.Categories, "search",
                    NewHttpUri().SetQueryParam("q", searchCriteria.Isbn).Query));
            }

            return pageableRequests;
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(SingleEpisodeSearchCriteria searchCriteria)
        {
            // Not supported for books
            return new IndexerPageableRequestChain();
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(SeasonSearchCriteria searchCriteria)
        {
            // Not supported for books
            return new IndexerPageableRequestChain();
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(DailyEpisodeSearchCriteria searchCriteria)
        {
            // Not supported for books
            return new IndexerPageableRequestChain();
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(DailySeasonSearchCriteria searchCriteria)
        {
            // Not supported for books
            return new IndexerPageableRequestChain();
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(AnimeEpisodeSearchCriteria searchCriteria)
        {
            // Not supported for books
            return new IndexerPageableRequestChain();
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(AnimeSeasonSearchCriteria searchCriteria)
        {
            // Not supported for books
            return new IndexerPageableRequestChain();
        }

        public virtual IndexerPageableRequestChain GetSearchRequests(SpecialEpisodeSearchCriteria searchCriteria)
        {
            // Not supported for books
            return new IndexerPageableRequestChain();
        }

        private void AddAuthorPageableRequests(IndexerPageableRequestChain chain, AuthorSearchCriteria searchCriteria)
        {
            chain.Add(GetPagedRequests(MaxPages, Settings.Categories, "search",
                NewHttpUri()
                    .SetQueryParam("author", searchCriteria.AuthorName)
                    .Query));
        }

        private IEnumerable<IndexerRequest> GetPagedRequests(int maxPages, IEnumerable<int> categories, string searchType, string parameters)
        {
            if (categories.Empty())
            {
                yield break;
            }

            var categoriesQuery = string.Join(",", categories.Distinct());

            var baseUrl = $"{Settings.BaseUrl.TrimEnd('/')}/{Settings.ApiPath.TrimEnd('/')}?t={searchType}&cat={categoriesQuery}&extended=1{Settings.AdditionalParameters}";

            if (Settings.ApiKey.IsNotNullOrWhiteSpace())
            {
                baseUrl += "&apikey=" + Settings.ApiKey;
            }

            if (PageSize == 0)
            {
                yield return new IndexerRequest(baseUrl + parameters, HttpAccept.Rss);
            }
            else
            {
                for (var page = 0; page < maxPages; page++)
                {
                    yield return new IndexerRequest($"{baseUrl}&offset={page * PageSize}&limit={PageSize}{parameters}", HttpAccept.Rss);
                }
            }
        }

        private static HttpUri NewHttpUri()
        {
            return new HttpUri("");
        }
    }
}