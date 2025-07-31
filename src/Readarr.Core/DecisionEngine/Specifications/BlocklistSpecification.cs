using NLog;
using Readarr.Core.Blocklisting;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class BlocklistSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly IBlocklistService _blocklistService;
        private readonly Logger _logger;

        public BlocklistSpecification(IBlocklistService blocklistService, Logger logger)
        {
            _blocklistService = blocklistService;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Database;
        public RejectionType Type => RejectionType.Permanent;

        public DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            if (_blocklistService.Blocklisted(subject.Author.Id, subject.Release))
            {
                _logger.Debug("{0} is blocklisted, rejecting.", subject.Release.Title);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.Blocklisted, "Release is blocklisted");
            }

            return DownloadSpecDecision.Accept();
        }

        public DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            if (_blocklistService.Blocklisted(subject.Series.Id, subject.Release))
            {
                _logger.Debug("{0} is blocklisted, rejecting.", subject.Release.Title);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.Blocklisted, "Release is blocklisted");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
