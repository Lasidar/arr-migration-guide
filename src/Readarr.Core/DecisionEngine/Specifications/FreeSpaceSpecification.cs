using System.IO;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Core.Configuration;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.DecisionEngine.Specifications
{
    public class FreeSpaceSpecification : IDualDownloadDecisionEngineSpecification
    {
        private readonly IConfigService _configService;
        private readonly IDiskProvider _diskProvider;
        private readonly Logger _logger;

        public FreeSpaceSpecification(IConfigService configService, IDiskProvider diskProvider, Logger logger)
        {
            _configService = configService;
            _diskProvider = diskProvider;
            _logger = logger;
        }

        public SpecificationPriority Priority => SpecificationPriority.Disk;
        public RejectionType Type => RejectionType.Permanent;

        public DownloadSpecDecision IsSatisfiedBy(RemoteBook subject, ReleaseDecisionInformation information)
        {
            return CheckFreeSpace(subject.Release.Size, subject.Author.Path);
        }

        public DownloadSpecDecision IsSatisfiedBy(RemoteEpisode subject, ReleaseDecisionInformation information)
        {
            return CheckFreeSpace(subject.Release.Size, subject.Series.Path);
        }

        private DownloadSpecDecision CheckFreeSpace(long size, string path)
        {
            if (_configService.SkipFreeSpaceCheckWhenImporting)
            {
                _logger.Debug("Skipping free space check");
                return DownloadSpecDecision.Accept();
            }
            long? freeSpace = null;

            try
            {
                freeSpace = _diskProvider.GetAvailableSpace(path);
            }
            catch (DirectoryNotFoundException)
            {
                // Ignore so it'll be skipped in the following checks
            }

            if (!freeSpace.HasValue)
            {
                _logger.Debug("Unable to get available space for {0}. Skipping", path);

                return DownloadSpecDecision.Accept();
            }

            var minimumSpace = _configService.MinimumFreeSpaceWhenImporting.Megabytes();
            var remainingSpace = freeSpace.Value - size;

            if (remainingSpace <= 0)
            {
                var message = "Importing after download will exceed available disk space";

                _logger.Debug(message);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.MinimumFreeSpace, message);
            }

            if (remainingSpace < minimumSpace)
            {
                var message = $"Not enough free space ({minimumSpace.SizeSuffix()}) to import after download: {remainingSpace.SizeSuffix()}. (Settings: Media Management: Minimum Free Space)";

                _logger.Debug(message);
                return DownloadSpecDecision.Reject(DownloadRejectionReason.MinimumFreeSpace, message);
            }

            return DownloadSpecDecision.Accept();
        }
    }
}
