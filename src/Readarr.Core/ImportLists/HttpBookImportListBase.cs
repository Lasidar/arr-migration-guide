using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentValidation.Results;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Common.Http;
using Readarr.Core.Configuration;
using Readarr.Core.Http;
using Readarr.Core.Localization;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists
{
    public abstract class HttpBookImportListBase<TSettings> : BookImportListBase<TSettings>
        where TSettings : IImportListSettings, new()
    {
        protected readonly IHttpClient _httpClient;

        public bool Enabled => true;
        public bool SupportsPaging => PageSize > 0;
        public virtual int PageSize => 0;
        public virtual TimeSpan RateLimit => TimeSpan.FromSeconds(2);

        public abstract IImportListRequestGenerator GetRequestGenerator();
        public abstract IParseImportListResponse GetParser();

        protected HttpBookImportListBase(IHttpClient httpClient,
            IImportListStatusService importListStatusService,
            IConfigService configService,
            IParsingService parsingService,
            ILocalizationService localizationService,
            Logger logger)
            : base(importListStatusService, configService, parsingService, localizationService, logger)
        {
            _httpClient = httpClient;
        }

        public override ImportListBookFetchResult Fetch()
        {
            return FetchItems(g => g.GetListItems(), true);
        }

        protected virtual ImportListBookFetchResult FetchItems(Func<IImportListRequestGenerator, ImportListPageableRequestChain> pageableRequestChainSelector, bool isRecent = false)
        {
            var releases = new List<ImportListBookInfo>();
            var anyFailure = false;

            var url = string.Empty;

            try
            {
                var generator = GetRequestGenerator();
                var parser = GetParser();

                var pageableRequestChain = pageableRequestChainSelector(generator);

                for (var i = 0; i < pageableRequestChain.Tiers; i++)
                {
                    var pageableRequests = pageableRequestChain.GetTier(i);

                    foreach (var pageableRequest in pageableRequests)
                    {
                        var pagedReleases = new List<ImportListBookInfo>();

                        foreach (var request in pageableRequest)
                        {
                            url = request.Url.FullUri;

                            var page = FetchPage(request, parser);

                            pagedReleases.AddRange(page);

                            if (pagedReleases.Count >= MaxNumberOfBooksToAdd && MaxNumberOfBooksToAdd > 0)
                            {
                                break;
                            }

                            if (!IsValidReleasesResponse(page))
                            {
                                anyFailure = true;
                                break;
                            }

                            if (SupportsPaging && page.Count >= PageSize)
                            {
                                continue;
                            }

                            break;
                        }

                        releases.AddRange(pagedReleases.Take(MaxNumberOfBooksToAdd));
                    }

                    if (releases.Any())
                    {
                        break;
                    }
                }

                _importListStatusService.RecordSuccess(Definition.Id);
            }
            catch (WebException webException)
            {
                anyFailure = true;

                if (webException.Status == WebExceptionStatus.NameResolutionFailure ||
                    webException.Status == WebExceptionStatus.ConnectFailure)
                {
                    _importListStatusService.RecordConnectionFailure(Definition.Id);
                }
                else
                {
                    _importListStatusService.RecordFailure(Definition.Id);
                }

                if (webException.Message.Contains("502") || webException.Message.Contains("503") ||
                    webException.Message.Contains("timed out"))
                {
                    _logger.Warn("{0} server is currently unavailable. {1} {2}", this, url, webException.Message);
                }
                else
                {
                    _logger.Warn("{0} {1} {2}", this, url, webException.Message);
                }
            }
            catch (TooManyRequestsException ex)
            {
                anyFailure = true;

                if (ex.RetryAfter != TimeSpan.Zero)
                {
                    _importListStatusService.RecordFailure(Definition.Id, ex.RetryAfter);
                }
                else
                {
                    _importListStatusService.RecordFailure(Definition.Id, TimeSpan.FromHours(1));
                }

                _logger.Warn("API Request Limit reached for {0}", this);
            }
            catch (HttpException ex)
            {
                anyFailure = true;
                _importListStatusService.RecordFailure(Definition.Id);

                _logger.Warn("{0} {1}", this, ex.Message);
            }
            catch (Exception ex)
            {
                anyFailure = true;
                _importListStatusService.RecordFailure(Definition.Id);

                _logger.Error(ex, "An error occurred while processing book list feed");
            }

            return new ImportListBookFetchResult(CleanupListItems(releases), anyFailure);
        }

        protected virtual IList<ImportListBookInfo> FetchPage(ImportListRequest request, IParseImportListResponse parser)
        {
            var response = FetchImportListResponse(request);

            return parser.ParseResponse(response).ToList();
        }

        protected virtual ImportListResponse FetchImportListResponse(ImportListRequest request)
        {
            _logger.Debug("Downloading Feed " + request.HttpRequest.ToString(false));

            if (request.HttpRequest.RateLimit < RateLimit)
            {
                request.HttpRequest.RateLimit = RateLimit;
            }

            return new ImportListResponse(request, _httpClient.Execute(request.HttpRequest));
        }

        protected virtual bool IsValidReleasesResponse(IList<ImportListBookInfo> releases)
        {
            return releases.Any();
        }

        protected virtual int MaxNumberOfBooksToAdd => 0;

        protected override void Test(List<ValidationFailure> failures)
        {
            failures.AddIfNotNull(TestConnection());
        }

        protected virtual ValidationFailure TestConnection()
        {
            try
            {
                var parser = GetParser();
                var generator = GetRequestGenerator();
                var releases = FetchPage(generator.GetListItems().GetAllTiers().First().First(), parser);

                if (releases.Empty())
                {
                    return new ValidationFailure(string.Empty, _localizationService.GetLocalizedString("ImportListTestNoResult"));
                }
            }
            catch (RequestLimitReachedException)
            {
                _logger.Warn("Request limit reached");
            }
            catch (UnsupportedFeedException ex)
            {
                _logger.Warn(ex, "Import list feed is not supported");

                return new ValidationFailure(string.Empty, _localizationService.GetLocalizedString("ImportListTestUnsupportedFeed"));
            }
            catch (ImportListException ex)
            {
                _logger.Warn(ex, "Unable to connect to import list");

                return new ValidationFailure(string.Empty, _localizationService.GetLocalizedString("ImportListTestLoadError", new Dictionary<string, object> { { "exceptionMessage", ex.Message } }));
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Unable to connect to import list");

                return new ValidationFailure(string.Empty, _localizationService.GetLocalizedString("ImportListTestUnknownError", new Dictionary<string, object> { { "exceptionMessage", ex.Message } }));
            }

            return null;
        }
    }
}