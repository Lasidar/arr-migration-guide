using NLog;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class QualityAllowedByProfileSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly Logger _logger;

        public QualityAllowedByProfileSpecification(Logger logger)
        {
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            _logger.Debug("Checking if report meets quality requirements. {0}", subject.Quality);

            var profile = subject.Author.QualityProfile.Value;
            var qualityIndex = profile.GetIndex(subject.Quality.Quality);
            var qualityOrGroup = profile.Items[qualityIndex.Index];

            if (!qualityOrGroup.Allowed)
            {
                _logger.Debug("Quality {0} rejected by Author's quality profile", subject.Quality);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.QualityNotWanted, "{0} is not wanted in profile", subject.Quality.Quality);
            }

            return DownloadSpecDecision.Accept();
        }

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            _logger.Debug("Checking if report meets quality requirements. {0}", subject.ParsedEpisodeInfo.Quality);

            var profile = subject.Series.QualityProfile.Value;
            var qualityIndex = profile.GetIndex(subject.ParsedEpisodeInfo.Quality.Quality);
            var qualityOrGroup = profile.Items[qualityIndex.Index];

            if (!qualityOrGroup.Allowed)
            {
                _logger.Debug("Quality {0} rejected by Series' quality profile", subject.ParsedEpisodeInfo.Quality);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.QualityNotWanted, "{0} is not wanted in profile", subject.ParsedEpisodeInfo.Quality.Quality);
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
