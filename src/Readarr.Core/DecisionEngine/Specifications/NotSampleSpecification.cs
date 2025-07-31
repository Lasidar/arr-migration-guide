using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class NotSampleSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly Logger _logger;

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public NotSampleSpecification(Logger logger)
        {
            _logger = logger;
        }

        public DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            return CheckIfSample(subject.Release);
        }

        public DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            return CheckIfSample(subject.Release);
        }

        private DownloadSpecDecision CheckIfSample(ReleaseInfo release)
        {
            if (release.Title.ToLower().Contains("sample") && release.Size < 70.Megabytes())
            {
                _logger.Debug("Sample release, rejecting.");
                return DownloadSpecDecision.Reject(DownloadRejectionReason.Sample, "Sample");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
