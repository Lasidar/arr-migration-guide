using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class AcceptableBookSizeSpecification : IBookDownloadDecisionEngineSpecification
    {
        private readonly Logger _logger;

        public AcceptableBookSizeSpecification(Logger logger)
        {
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Default;
        public RejectionType Type => RejectionType.Permanent;

        public DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            _logger.Debug("Beginning size check for: {0}", subject);

            var quality = subject.Quality.Quality;

            if (subject.Release.Size == 0)
            {
                _logger.Debug("Release has unknown size, skipping size check");
                return DownloadSpecDecision.Accept();
            }

            var qualityProfile = subject.Author.QualityProfile.Value;
            var qualityIndex = qualityProfile.GetIndex(quality, true);
            var qualityOrGroup = qualityProfile.Items[qualityIndex.Index];
            var item = qualityOrGroup.Quality == null ? qualityOrGroup.Items[qualityIndex.GroupIndex] : qualityOrGroup;

            if (item.MinSize.HasValue)
            {
                var minSize = item.MinSize.Value.Megabytes();

                // If the parsed size is smaller than minSize we don't want it
                if (subject.Release.Size < minSize)
                {
                    _logger.Debug("Item: {0}, Size: {1} is smaller than minimum allowed size ({2} bytes), rejecting.", 
                        subject, subject.Release.Size, minSize);
                    return DownloadSpecDecision.Reject(DownloadRejectionReason.BelowMinimumSize, 
                        "{0} is smaller than minimum allowed {1}", 
                        subject.Release.Size.SizeSuffix(), minSize.SizeSuffix());
                }
            }

            if (!item.MaxSize.HasValue || item.MaxSize.Value == 0)
            {
                _logger.Debug("Max size is unlimited, skipping size check");
            }
            else
            {
                var maxSize = item.MaxSize.Value.Megabytes();

                // If the parsed size is greater than maxSize we don't want it
                if (subject.Release.Size > maxSize)
                {
                    _logger.Debug("Item: {0}, Size: {1} is greater than maximum allowed size ({2}), rejecting", 
                        subject, subject.Release.Size, maxSize);
                    return DownloadSpecDecision.Reject(DownloadRejectionReason.AboveMaximumSize, 
                        "{0} is larger than maximum allowed {1}", 
                        subject.Release.Size.SizeSuffix(), maxSize.SizeSuffix());
                }
            }

            _logger.Debug("Item: {0}, meets size constraints", subject);
            return DownloadSpecDecision.Accept();
        }
    }
}