using System;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Configuration;
using Readarr.Core.CustomFormats;
using Readarr.Core.History;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications.RssSync
{
    public class BookHistorySpecification : IBookDownloadDecisionEngineSpecification
    {
        private readonly IBookHistoryService _historyService;
        private readonly UpgradableSpecification _upgradableSpecification;
        private readonly ICustomFormatCalculationService _formatService;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public BookHistorySpecification(IBookHistoryService historyService,
                                       UpgradableSpecification upgradableSpecification,
                                       ICustomFormatCalculationService formatService,
                                       IConfigService configService,
                                       Logger logger)
        {
            _historyService = historyService;
            _upgradableSpecification = upgradableSpecification;
            _formatService = formatService;
            _configService = configService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Database;
        public RejectionType Type => RejectionType.Permanent;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            if (information.SearchCriteria != null)
            {
                _logger.Debug("Skipping history check during search");
                return DownloadSpecDecision.Accept();
            }

            var cdhEnabled = _configService.EnableCompletedDownloadHandling;
            var qualityProfile = subject.Author.QualityProfile.Value;

            _logger.Debug("Performing history status check on report");

            foreach (var book in subject.Books)
            {
                _logger.Debug("Checking current status of book [{0}] in history", book.Id);
                var mostRecent = _historyService.MostRecentForBook(book.Id);

                if (mostRecent != null && mostRecent.EventType == HistoryEventType.Grabbed)
                {
                    var recent = mostRecent.Date.After(DateTime.UtcNow.AddHours(-12));

                    if (!recent && cdhEnabled)
                    {
                        continue;
                    }

                    var customFormats = _formatService.ParseCustomFormat(mostRecent, subject.Author);
                    var cutoffUnmet = _upgradableSpecification.CutoffNotMet(
                        qualityProfile,
                        mostRecent.Quality,
                        customFormats,
                        mostRecent.Languages);

                    var upgradeable = _upgradableSpecification.IsUpgradable(
                        qualityProfile,
                        mostRecent.Quality,
                        customFormats,
                        subject.ParsedBookInfo.Quality,
                        subject.CustomFormats,
                        mostRecent.Languages,
                        subject.ParsedBookInfo.Languages);

                    if (!cutoffUnmet)
                    {
                        if (upgradeable)
                        {
                            _logger.Debug("Book [{0}] was grabbed recently and quality is upgradeable, skipping", book.Id);
                            return DownloadSpecDecision.Reject("Recent grab event in history already meets cutoff: {0}", mostRecent.Quality);
                        }

                        _logger.Debug("Book [{0}] was grabbed recently and quality is not upgradeable, skipping", book.Id);
                        return DownloadSpecDecision.Reject("Recent grab event in history already meets cutoff: {0}", mostRecent.Quality);
                    }
                }
            }

            return DownloadSpecDecision.Accept();
        }
    }
}