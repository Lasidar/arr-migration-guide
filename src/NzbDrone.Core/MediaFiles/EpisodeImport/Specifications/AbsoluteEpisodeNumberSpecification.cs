using System;
using NLog;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Download;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MediaFiles.EpisodeImport.Specifications
{
    public class AbsoluteEditionNumberSpecification : IImportDecisionEngineSpecification
    {
        private readonly IBuildFileNames _buildFileNames;
        private readonly Logger _logger;

        public AbsoluteEditionNumberSpecification(IBuildFileNames buildFileNames, Logger logger)
        {
            _buildFileNames = buildFileNames;
            _logger = logger;
        }

        public ImportSpecDecision IsSatisfiedBy(LocalEpisode localEpisode, DownloadClientItem downloadClientItem)
        {
            if (localEpisode.Series.SeriesType != SeriesTypes.Anime)
            {
                _logger.Debug("Series type is not Anime, skipping check");
                return ImportSpecDecision.Accept();
            }

            if (!_buildFileNames.RequiresAbsoluteEditionNumber())
            {
                _logger.Debug("File name format does not require absolute episode number, skipping check");
                return ImportSpecDecision.Accept();
            }

            foreach (var episode in localEpisode.Episodes)
            {
                var airDateUtc = episode.AirDateUtc;
                var absoluteEditionNumber = episode.AbsoluteEditionNumber;

                if (airDateUtc.HasValue && airDateUtc.Value.Before(DateTime.UtcNow.AddDays(-1)))
                {
                    _logger.Debug("Episode aired more than 1 day ago");
                    continue;
                }

                if (!absoluteEditionNumber.HasValue)
                {
                    _logger.Debug("Episode does not have an absolute episode number and recently aired");

                    return ImportSpecDecision.Reject(ImportRejectionReason.MissingAbsoluteEditionNumber, "Episode does not have an absolute episode number and recently aired");
                }
            }

            return ImportSpecDecision.Accept();
        }
    }
}
