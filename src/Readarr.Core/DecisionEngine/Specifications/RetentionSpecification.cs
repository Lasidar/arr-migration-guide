using NLog;
using Readarr.Core.Configuration;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class RetentionSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public RetentionSpecification(IConfigService configService, Logger logger)
        {
            _configService = configService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            return CheckRetention(subject.Release);
        }

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            return CheckRetention(subject.Release);
        }

        private DownloadSpecDecision CheckRetention(ReleaseInfo release)
        {
            if (release.DownloadProtocol != Indexers.DownloadProtocol.Usenet)
            {
                _logger.Debug("Not checking retention requirement for non-usenet report");
                return DownloadSpecDecision.Accept();
            }

            var age = release.Age;
            var retention = _configService.Retention;

            _logger.Debug("Checking if report meets retention requirements. {0}", age);
            if (retention > 0 && age > retention)
            {
                _logger.Debug("Report age: {0} rejected by user's retention limit", age);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.MaximumAge, "Older than configured retention");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
