using System.Linq;
using NLog;
using Readarr.Core.Configuration;
using Readarr.Core.CustomFormats;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Core.Queue;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class BookQueueSpecification : IBookDownloadDecisionEngineSpecification
    {
        private readonly IQueueService _queueService;
        private readonly UpgradableSpecification _upgradableSpecification;
        private readonly ICustomFormatCalculationService _formatService;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public BookQueueSpecification(IQueueService queueService,
                                     UpgradableSpecification upgradableSpecification,
                                     ICustomFormatCalculationService formatService,
                                     IConfigService configService,
                                     Logger logger)
        {
            _queueService = queueService;
            _upgradableSpecification = upgradableSpecification;
            _formatService = formatService;
            _configService = configService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            var queue = _queueService.GetQueue();
            var matchingBook = queue.Where(q => q.RemoteBook?.Author != null &&
                                                q.RemoteBook.Author.Id == subject.Author.Id &&
                                                q.RemoteBook.Books.Select(b => b.Id).Intersect(subject.Books.Select(b => b.Id)).Any())
                                    .ToList();

            foreach (var queueItem in matchingBook)
            {
                var remoteBook = queueItem.RemoteBook;
                var qualityProfile = subject.Author.QualityProfile.Value;

                // To avoid a race make sure it's not FailedPending (failed awaiting removal/search).
                // Failed items (already searching for a replacement) won't be part of the queue since
                // it's a copy, but the ping could happen before it's removed. Subsequent pings will
                // exclude the failed item.

                if (queueItem.TrackedDownloadState == TrackedDownloadState.FailedPending)
                {
                    continue;
                }

                _logger.Debug("Checking if existing release in queue meets cutoff. Queued quality is: {0} - {1}",
                              remoteBook.ParsedBookInfo.Quality,
                              remoteBook.CustomFormats.ConcatToString());

                var queuedItemCustomFormats = _formatService.ParseCustomFormat(remoteBook, subject.Author);

                if (!_upgradableSpecification.CutoffNotMet(qualityProfile,
                                                           remoteBook.ParsedBookInfo.Quality,
                                                           queuedItemCustomFormats,
                                                           remoteBook.ParsedBookInfo.Languages))
                {
                    return DownloadSpecDecision.Reject("Release in queue already meets cutoff: {0} - {1}",
                                                      remoteBook.ParsedBookInfo.Quality,
                                                      remoteBook.CustomFormats.ConcatToString());
                }

                _logger.Debug("Checking if release is higher quality than queued release. Queued: {0} - {1}",
                              remoteBook.ParsedBookInfo.Quality,
                              remoteBook.CustomFormats.ConcatToString());

                if (!_upgradableSpecification.IsUpgradable(qualityProfile,
                                                          remoteBook.ParsedBookInfo.Quality,
                                                          queuedItemCustomFormats,
                                                          subject.ParsedBookInfo.Quality,
                                                          subject.CustomFormats,
                                                          remoteBook.ParsedBookInfo.Languages,
                                                          subject.ParsedBookInfo.Languages))
                {
                    return DownloadSpecDecision.Reject("Release in queue is of equal or higher quality: {0} - {1}",
                                                      remoteBook.ParsedBookInfo.Quality,
                                                      remoteBook.CustomFormats.ConcatToString());
                }

                _logger.Debug("Checking if profiles allow upgrading. Queued: {0} - {1}",
                              remoteBook.ParsedBookInfo.Quality,
                              remoteBook.CustomFormats.ConcatToString());

                if (!_upgradableSpecification.IsUpgradeAllowed(qualityProfile,
                                                               remoteBook.ParsedBookInfo.Quality,
                                                               queuedItemCustomFormats,
                                                               subject.ParsedBookInfo.Quality,
                                                               subject.CustomFormats,
                                                               remoteBook.ParsedBookInfo.Languages,
                                                               subject.ParsedBookInfo.Languages))
                {
                    return DownloadSpecDecision.Reject("Another release is queued and the Quality profile does not allow upgrades");
                }
            }

            return DownloadSpecDecision.Accept();
        }
    }
}