using System;
using NLog;
using Readarr.Core.Configuration;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class MinimumAgeSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public MinimumAgeSpecification(IConfigService configService, Logger logger)
        {
            _configService = configService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Temporary;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            return CheckAge(subject.Release);
        }

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            return CheckAge(subject.Release);
        }

        private DownloadSpecDecision CheckAge(ReleaseInfo release)
        {
            if (release.DownloadProtocol != Indexers.DownloadProtocol.Usenet)
            {
                _logger.Debug("Not checking minimum age requirement for non-usenet report");
                return DownloadSpecDecision.Accept();
            }

            var age = release.AgeMinutes;
            var minimumAge = _configService.MinimumAge;
            var ageRounded = Math.Round(age, 1);

            if (minimumAge == 0)
            {
                _logger.Debug("Minimum age is not set.");
                return DownloadSpecDecision.Accept();
            }

            _logger.Debug("Checking if report meets minimum age requirements. {0}", ageRounded);

            if (age < minimumAge)
            {
                _logger.Debug("Only {0} minutes old, minimum age is {1} minutes", ageRounded, minimumAge);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.MinimumAge, "Only {0} minutes old, minimum age is {1} minutes", ageRounded, minimumAge);
            }

            _logger.Debug("Release is {0} minutes old, greater than minimum age of {1} minutes", ageRounded, minimumAge);

            return DownloadSpecDecision.Accept();
        }
    }
}
