using System.Linq;
using NLog;
using Readarr.Core.CustomFormats;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class UpgradeAllowedSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly UpgradableSpecification _upgradableSpecification;
        private readonly ICustomFormatCalculationService _formatService;
        private readonly Logger _logger;

        public UpgradeAllowedSpecification(UpgradableSpecification upgradableSpecification,
                                           ICustomFormatCalculationService formatService,
                                           Logger logger)
        {
            _upgradableSpecification = upgradableSpecification;
            _formatService = formatService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            var qualityProfile = subject.Author.QualityProfile.Value;

            foreach (var book in subject.Books.Where(b => b.BookFiles.Value.Any()))
            {
                var file = book.BookFiles.Value.First();
                
                if (file == null)
                {
                    _logger.Debug("File is no longer available, skipping this file.");
                    continue;
                }

                var fileCustomFormats = _formatService.ParseCustomFormat(file, subject.Author);

                _logger.Debug("Comparing file quality with report. Existing file is {0}", file.Quality);

                if (!_upgradableSpecification.IsUpgradeAllowed(qualityProfile,
                                                               file.Quality,
                                                               fileCustomFormats,
                                                               subject.Quality,
                                                               subject.CustomFormats))
                {
                    _logger.Debug("Upgrading is not allowed by the quality profile");

                    return DownloadSpecDecision.Reject(DownloadRejectionReason.QualityUpgradesDisabled, "Existing file and the Quality profile does not allow upgrades");
                }
            }

            return DownloadSpecDecision.Accept();
        }

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            var qualityProfile = subject.Series.QualityProfile.Value;

            foreach (var file in subject.Episodes.Where(c => c.EpisodeFileId != 0).Select(c => c.EpisodeFile.Value))
            {
                if (file == null)
                {
                    _logger.Debug("File is no longer available, skipping this file.");
                    continue;
                }

                var fileCustomFormats = _formatService.ParseCustomFormat(file, subject.Series);

                _logger.Debug("Comparing file quality with report. Existing file is {0}", file.Quality);

                if (!_upgradableSpecification.IsUpgradeAllowed(qualityProfile,
                                                               file.Quality,
                                                               fileCustomFormats,
                                                               subject.ParsedEpisodeInfo.Quality,
                                                               subject.CustomFormats))
                {
                    _logger.Debug("Upgrading is not allowed by the quality profile");

                    return DownloadSpecDecision.Reject(DownloadRejectionReason.QualityUpgradesDisabled, "Existing file and the Quality profile does not allow upgrades");
                }
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
