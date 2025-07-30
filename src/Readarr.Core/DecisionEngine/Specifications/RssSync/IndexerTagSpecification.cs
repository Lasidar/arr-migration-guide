using System.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore;
using Readarr.Core.Indexers;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications.RssSync
{
    public class IndexerTagSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly Logger _logger;
        private readonly IIndexerFactory _indexerFactory;

        public IndexerTagSpecification(Logger logger, IIndexerFactory indexerFactory)
        {
            _logger = logger;
            _indexerFactory = indexerFactory;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            if (subject.Release == null || subject.Author?.Tags == null || subject.Release.IndexerId == 0)
            {
                return DownloadSpecDecision.Accept();
            }

            IndexerDefinition indexer;
            try
            {
                indexer = _indexerFactory.Get(subject.Release.IndexerId);
            }
            catch (ModelNotFoundException)
            {
                _logger.Debug("Indexer with id {0} does not exist, skipping indexer tags check", subject.Release.IndexerId);
                return DownloadSpecDecision.Accept();
            }

            // If indexer has tags, check that at least one of them is present on the author
            var indexerTags = indexer.Tags;

            if (indexerTags.Any() && indexerTags.Intersect(subject.Author.Tags).Empty())
            {
                _logger.Debug("Indexer {0} has tags. None of these are present on author {1}. Rejecting", subject.Release.Indexer, subject.Author);

                return DownloadSpecDecision.Reject(DownloadRejectionReason.NoMatchingTag, "Author tags do not match any of the indexer tags");
            }

            return DownloadSpecDecision.Accept();
        }

        public virtual DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            if (subject.Release == null || subject.Series?.Tags == null || subject.Release.IndexerId == 0)
            {
                return DownloadSpecDecision.Accept();
            }

            IndexerDefinition indexer;
            try
            {
                indexer = _indexerFactory.Get(subject.Release.IndexerId);
            }
            catch (ModelNotFoundException)
            {
                _logger.Debug("Indexer with id {0} does not exist, skipping indexer tags check", subject.Release.IndexerId);
                return DownloadSpecDecision.Accept();
            }

            // If indexer has tags, check that at least one of them is present on the series
            var indexerTags = indexer.Tags;

            if (indexerTags.Any() && indexerTags.Intersect(subject.Series.Tags).Empty())
            {
                _logger.Debug("Indexer {0} has tags. None of these are present on series {1}. Rejecting", subject.Release.Indexer, subject.Series);

                return DownloadSpecDecision.Reject(DownloadRejectionReason.NoMatchingTag, "Series tags do not match any of the indexer tags");
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
