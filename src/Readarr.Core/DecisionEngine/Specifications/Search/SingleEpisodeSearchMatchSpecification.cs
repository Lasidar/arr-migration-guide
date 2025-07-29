using System.Linq;
using NLog;
using Readarr.Core.DataAugmentation.Scene;
using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications.Search
{
    public class SingleEpisodeSearchMatchSpecification : IDownloadDecisionEngineSpecification
    {
        private readonly Logger _logger;
        private readonly ISceneMappingService _sceneMappingService;

        public SingleEpisodeSearchMatchSpecification(ISceneMappingService sceneMappingService, Logger logger)
        {
            _logger = logger;
            _sceneMappingService = sceneMappingService;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public DownloadSpecDecision IsSatisfiedBy(RemoteEpisode remoteEpisode, ReleaseDecisionInformation information)
        {
            var searchCriteria = information.SearchCriteria;

            if (searchCriteria == null)
            {
                return DownloadSpecDecision.Accept();
            }

            if (searchCriteria is SingleEpisodeSearchCriteria singleEpisodeSpec)
            {
                return IsSatisfiedBy(remoteEpisode, singleEpisodeSpec);
            }

            if (searchCriteria is AnimeEpisodeSearchCriteria animeEpisodeSpec)
            {
                return IsSatisfiedBy(remoteEpisode, animeEpisodeSpec);
            }

            return DownloadSpecDecision.Accept();
        }

        private DownloadSpecDecision IsSatisfiedBy(RemoteEpisode remoteEpisode, SingleEpisodeSearchCriteria singleEpisodeSpec)
        {
            if (singleEpisodeSpec.SeasonNumber != remoteEpisode.ParsedEpisodeInfo.SeasonNumber)
            {
                _logger.Debug("Season number does not match searched season number, skipping.");
                return DownloadSpecDecision.Reject(DownloadRejectionReason.WrongSeason, "Wrong season");
            }

            if (!remoteEpisode.ParsedEpisodeInfo.EpisodeNumbers.Any())
            {
                _logger.Debug("Full season result during single episode search, skipping.");
                return DownloadSpecDecision.Reject(DownloadRejectionReason.FullSeason, "Full season pack");
            }

            if (!remoteEpisode.ParsedEpisodeInfo.EpisodeNumbers.Contains(singleEpisodeSpec.EpisodeNumber))
            {
                _logger.Debug("Episode number does not match searched episode number, skipping.");
                return DownloadSpecDecision.Reject(DownloadRejectionReason.WrongEpisode, "Wrong episode");
            }

            return DownloadSpecDecision.Accept();
        }

        private DownloadSpecDecision IsSatisfiedBy(RemoteEpisode remoteEpisode, AnimeEpisodeSearchCriteria animeEpisodeSpec)
        {
            if (remoteEpisode.ParsedEpisodeInfo.FullSeason && !animeEpisodeSpec.IsSeasonSearch)
            {
                _logger.Debug("Full season result during single episode search, skipping.");
                return DownloadSpecDecision.Reject(DownloadRejectionReason.FullSeason, "Full season pack");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
