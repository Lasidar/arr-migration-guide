using System;
using System.Linq;
using NLog;
using Readarr.Core.Configuration;
using Readarr.Core.History;
using Readarr.Core.Indexers;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class AlreadyImportedBookSpecification : IBookDownloadDecisionEngineSpecification
    {
        private readonly IBookHistoryService _historyService;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public AlreadyImportedBookSpecification(IBookHistoryService historyService,
                                                IConfigService configService,
                                                Logger logger)
        {
            _historyService = historyService;
            _configService = configService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Database;
        public RejectionType Type => RejectionType.Permanent;

        public DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            var cdhEnabled = _configService.EnableCompletedDownloadHandling;

            if (!cdhEnabled)
            {
                _logger.Debug("Skipping already imported check because CDH is disabled");
                return DownloadSpecDecision.Accept();
            }

            _logger.Debug("Performing already imported check on report");
            foreach (var book in subject.Books)
            {
                if (!book.AnyEditionOk)
                {
                    _logger.Debug("Skipping already imported check for book without file");
                    continue;
                }

                var historyForBook = _historyService.FindByBookId(book.Id);
                var lastGrabbed = historyForBook.FirstOrDefault(h => h.EventType == HistoryEventType.Grabbed);

                if (lastGrabbed == null)
                {
                    continue;
                }

                var imported = historyForBook.FirstOrDefault(h =>
                    h.EventType == HistoryEventType.DownloadFolderImported &&
                    h.DownloadId == lastGrabbed.DownloadId);

                if (imported == null)
                {
                    continue;
                }

                // This is really only a guard against redownloading the same release over and over
                // when the grabbed and imported qualities do not match, if they do or if the release
                // was imported from a file manually this will still apply.

                if (lastGrabbed.SourceTitle == subject.Release.Title)
                {
                    _logger.Debug("Has same release name as a grabbed and imported release");
                    return DownloadSpecDecision.Reject("Has same release name as a grabbed and imported release");
                }

                // Only based on book file size is enough for a match, to make it work with manual imports
                if (Math.Abs(lastGrabbed.Data.GetValueOrDefault("size", 0.0) - subject.Release.Size) < 10000)
                {
                    _logger.Debug("Has same release size as a grabbed and imported release");
                    return DownloadSpecDecision.Reject("Has same release size as a grabbed and imported release");
                }
            }

            return DownloadSpecDecision.Accept();
        }
    }
}