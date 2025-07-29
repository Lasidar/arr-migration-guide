using NLog;
using Readarr.Core.DecisionEngine;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.EpisodeImport.Specifications
{
    public class SameEpisodesImportSpecification : IImportDecisionEngineSpecification
    {
        private readonly SameEpisodesSpecification _sameEpisodesSpecification;
        private readonly Logger _logger;

        public SameEpisodesImportSpecification(SameEpisodesSpecification sameEpisodesSpecification, Logger logger)
        {
            _sameEpisodesSpecification = sameEpisodesSpecification;
            _logger = logger;
        }

        public RejectionType Type => RejectionType.Permanent;

        public ImportSpecDecision IsSatisfiedBy(LocalEpisode localEpisode, DownloadClientItem downloadClientItem)
        {
            if (_sameEpisodesSpecification.IsSatisfiedBy(localEpisode.Episodes))
            {
                return ImportSpecDecision.Accept();
            }

            _logger.Debug("Episode file on disk contains more episodes than this file contains");
            return ImportSpecDecision.Reject(ImportRejectionReason.ExistingFileHasMoreEpisodes, "Episode file on disk contains more episodes than this file contains");
        }
    }
}
