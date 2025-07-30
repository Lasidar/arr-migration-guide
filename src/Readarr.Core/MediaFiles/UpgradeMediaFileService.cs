using System.IO;
using System.Linq;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Core.MediaFiles.BookImport;
using Readarr.Core.MediaFiles.EpisodeImport;
using Readarr.Core.Parser.Model;
using System;
using System.Collections.Generic;

namespace Readarr.Core.MediaFiles
{
    public class UpgradeMediaFileService : IUpgradeMediaFiles
    {
        private readonly IRecycleBinProvider _recycleBinProvider;
        private readonly IMediaFileService _mediaFileService;
        private readonly IMoveEpisodeFiles _episodeFileMover;
        private readonly IDiskProvider _diskProvider;
        private readonly Logger _logger;

        public UpgradeMediaFileService(IRecycleBinProvider recycleBinProvider,
                                       IMediaFileService mediaFileService,
                                       IMoveEpisodeFiles episodeFileMover,
                                       IDiskProvider diskProvider,
                                       Logger logger)
        {
            _recycleBinProvider = recycleBinProvider;
            _mediaFileService = mediaFileService;
            _episodeFileMover = episodeFileMover;
            _diskProvider = diskProvider;
            _logger = logger;
        }

        public EpisodeFileUpgradeResult UpgradeEpisodeFile(EpisodeFile episodeFile, LocalEpisode localEpisode, bool copyOnly = false)
        {
            // TODO: This is a temporary implementation for TV compatibility
            // Should be removed when TV-specific code is fully migrated
            var result = new EpisodeFileUpgradeResult
            {
                EpisodeFile = episodeFile,
                OldFiles = new List<EpisodeFile>()
            };

            var existingFiles = localEpisode.Episodes
                .SelectMany(e => _mediaFileService.GetFilesByEpisodeIds(new List<int> { e.Id }))
                .GroupBy(e => e.Id)
                .Select(g => g.First())
                .ToList();

            result.OldFiles.AddRange(existingFiles);

            // Move or copy the file
            episodeFile = _episodeFileMover.MoveEpisodeFile(episodeFile, localEpisode);

            // Delete old files if not copying
            if (!copyOnly)
            {
                _mediaFileService.DeleteMany(existingFiles, DeleteMediaFileReason.Upgrade);
            }

            result.EpisodeFile = episodeFile;
            return result;
        }

        // Book interface implementation (stub for now)
        public BookFileUpgradeResult UpgradeBookFile(BookFile bookFile, LocalBook localBook, bool copyOnly = false)
        {
            // TODO: Implement book file upgrade
            throw new NotImplementedException();
        }
    }
}
