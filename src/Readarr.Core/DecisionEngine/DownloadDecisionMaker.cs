using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Common.Instrumentation.Extensions;
using Readarr.Common.Serializer;
using Readarr.Core.Configuration;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine
{
    public interface IDownloadDecisionMaker
    {
        List<DownloadDecision> GetRssDecision(List<ReleaseInfo> reports);
        List<DownloadDecision> GetSearchDecision(List<ReleaseInfo> reports, SearchCriteriaBase searchCriteriaBase);
    }

    public class DownloadDecisionMaker : IDownloadDecisionMaker
    {
        private readonly IEnumerable<IDecisionEngineSpecification> _specifications;
        private readonly IParsingService _parsingService;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public DownloadDecisionMaker(IEnumerable<IDecisionEngineSpecification> specifications,
                                    IParsingService parsingService,
                                    IConfigService configService,
                                    Logger logger)
        {
            _specifications = specifications;
            _parsingService = parsingService;
            _configService = configService;
            _logger = logger;
        }

        public List<DownloadDecision> GetRssDecision(List<ReleaseInfo> reports)
        {
            return GetDecisions(reports).ToList();
        }

        public List<DownloadDecision> GetSearchDecision(List<ReleaseInfo> reports, SearchCriteriaBase searchCriteriaBase)
        {
            return GetDecisions(reports, searchCriteriaBase).ToList();
        }

        private IEnumerable<DownloadDecision> GetDecisions(List<ReleaseInfo> reports, SearchCriteriaBase searchCriteria = null)
        {
            if (reports.Any())
            {
                _logger.ProgressInfo("Processing {0} releases", reports.Count);
            }
            else
            {
                _logger.ProgressInfo("No results found");
            }

            var reportNumber = 1;

            foreach (var report in reports)
            {
                DownloadDecision decision = null;
                _logger.ProgressTrace("Processing release {0}/{1}", reportNumber, reports.Count);
                _logger.Debug("Processing release '{0}' from '{1}'", report.Title, report.Indexer);

                try
                {
                    var parsedBookInfo = Parser.Parser.ParseBookTitle(report.Title);

                    if (parsedBookInfo == null || parsedBookInfo.BookTitle.IsNullOrWhiteSpace())
                    {
                        _logger.Debug("Unable to parse release title");
                        parsedBookInfo = new BookInfo
                        {
                            BookTitle = report.Title
                        };
                    }

                    var remoteBook = _parsingService.Map(parsedBookInfo, searchCriteria);

                    // Ensure that the parsed book title matches
                    if (remoteBook.Author == null)
                    {
                        _logger.Debug("Couldn't find author in {0}", report.Title);
                        
                        decision = new DownloadDecision(remoteBook, new Rejection("Unknown Author"));
                    }
                    else if (remoteBook.Books.Empty())
                    {
                        _logger.Debug("Couldn't find book in {0}", report.Title);
                        
                        decision = new DownloadDecision(remoteBook, new Rejection("Unknown Book"));
                    }
                    else
                    {
                        remoteBook.Release = report;

                        if (remoteBook.Author == null)
                        {
                            decision = GetDecisionForReport(remoteBook, searchCriteria);
                        }
                        else
                        {
                            // Check if any book matches the search criteria
                            if (searchCriteria != null)
                            {
                                if (!MatchesSearchCriteria(remoteBook, searchCriteria))
                                {
                                    decision = new DownloadDecision(remoteBook, new Rejection("Book does not match search criteria"));
                                }
                            }

                            if (decision == null)
                            {
                                decision = GetDecisionForReport(remoteBook, searchCriteria);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Couldn't process release {0}", report.Title);

                    var remoteBook = new RemoteBook { Release = report };
                    decision = new DownloadDecision(remoteBook, new Rejection("Unexpected error processing release"));
                }

                reportNumber++;

                if (decision != null)
                {
                    var source = report.Indexer;
                    
                    if (searchCriteria != null)
                    {
                        if (searchCriteria.InteractiveSearch)
                        {
                            source = "Interactive Search";
                        }
                        else if (searchCriteria.UserInvokedSearch)
                        {
                            source = "User Search";
                        }
                    }

                    if (decision.Rejections.Any())
                    {
                        _logger.Debug("Release '{0}' from '{1}' rejected for the following reasons: {2}",
                            report.Title,
                            source,
                            string.Join(", ", decision.Rejections));
                    }
                    else
                    {
                        _logger.Debug("Release '{0}' from '{1}' accepted", report.Title, source);
                    }

                    yield return decision;
                }
            }
        }

        private DownloadDecision GetDecisionForReport(RemoteBook remoteBook, SearchCriteriaBase searchCriteria = null)
        {
            var reasons = new Rejection[0];

            foreach (var specification in _specifications)
            {
                try
                {
                    var result = specification.IsSatisfiedBy(remoteBook, searchCriteria);

                    if (!result.Accepted)
                    {
                        reasons = reasons.Union(result.Rejections).ToArray();
                    }
                }
                catch (NotImplementedException)
                {
                    _logger.Trace("Spec {0} not implemented", specification.GetType().Name);
                }
                catch (Exception e)
                {
                    e.Data.Add("report", remoteBook.Release.ToJson());
                    e.Data.Add("parsed", remoteBook.ParsedBookInfo.ToJson());
                    _logger.Error(e, "Couldn't evaluate decision on '{0}'", remoteBook.Release.Title);
                    return new DownloadDecision(remoteBook, new Rejection($"{specification.GetType().Name}: {e.Message}"));
                }
            }

            return new DownloadDecision(remoteBook, reasons.ToArray());
        }

        private bool MatchesSearchCriteria(RemoteBook remoteBook, SearchCriteriaBase searchCriteria)
        {
            if (searchCriteria is BookSearchCriteria bookSearch)
            {
                if (bookSearch.Author != null && remoteBook.Author.Id != bookSearch.Author.Id)
                {
                    return false;
                }

                if (bookSearch.Books.Any() && !remoteBook.Books.Any(b => bookSearch.Books.Any(sb => sb.Id == b.Id)))
                {
                    return false;
                }
            }
            else if (searchCriteria is AuthorSearchCriteria authorSearch)
            {
                if (authorSearch.AuthorName.IsNotNullOrWhiteSpace() && 
                    !remoteBook.Author.Name.ContainsIgnoreCase(authorSearch.AuthorName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
