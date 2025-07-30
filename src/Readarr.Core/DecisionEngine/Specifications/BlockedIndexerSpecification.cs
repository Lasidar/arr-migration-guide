using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Common.Cache;
using Readarr.Core.Indexers;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class BlockedIndexerSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly IIndexerStatusService _indexerStatusService;
        private readonly Logger _logger;

        private readonly ICachedDictionary<IndexerStatus> _blockedIndexerCache;

        public BlockedIndexerSpecification(IIndexerStatusService indexerStatusService, ICacheManager cacheManager, Logger logger)
        {
            _indexerStatusService = indexerStatusService;
            _logger = logger;

            _blockedIndexerCache = cacheManager.GetCacheDictionary(GetType(), "blocked", FetchBlockedIndexer, TimeSpan.FromSeconds(15));
        }

        public SpecificationPriority Priority => SpecificationPriority.Database;
        public RejectionType Type => RejectionType.Temporary;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            return CheckIndexerStatus(subject.Release);
        }

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            return CheckIndexerStatus(subject.Release);
        }

        private DownloadSpecDecision CheckIndexerStatus(ReleaseInfo release)
        {
            var status = _blockedIndexerCache.Find(release.IndexerId.ToString());
            if (status != null)
            {
                return DownloadSpecDecision.Reject(DownloadRejectionReason.IndexerDisabled, $"Indexer {release.Indexer} is blocked till {status.DisabledTill} due to failures, cannot grab release.");
            }

            return DownloadSpecDecision.Accept();
        }

        private IDictionary<string, IndexerStatus> FetchBlockedIndexer()
        {
            return _indexerStatusService.GetBlockedProviders().ToDictionary(v => v.ProviderId.ToString());
        }
    }
}
