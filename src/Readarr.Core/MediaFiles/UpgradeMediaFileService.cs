using System.IO;
using System.Linq;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Core.MediaFiles.BookImport;
using Readarr.Core.MediaFiles.BookImport;
using Readarr.Core.Parser.Model;
using System;
using System.Collections.Generic;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaFiles
{
    public class UpgradeMediaFileService : IUpgradeMediaFiles
    {
        private readonly IRecycleBinProvider _recycleBinProvider;
        private readonly IMediaFileService _mediaFileService;
        private readonly IMoveEpisodeFiles _episodeFileMover;
        private readonly IDiskProvider _diskProvider;
        private readonly IMediaFileRepository _episodeFileRepository;
        private readonly Logger _logger;

        public UpgradeMediaFileService(IRecycleBinProvider recycleBinProvider,
                                       IMediaFileService mediaFileService,
                                       IMoveEpisodeFiles episodeFileMover,
                                       IDiskProvider diskProvider,
                                       IMediaFileRepository episodeFileRepository,
                                       Logger logger)
        {
            _recycleBinProvider = recycleBinProvider;
            _mediaFileService = mediaFileService;
            _episodeFileMover = episodeFileMover;
            _diskProvider = diskProvider;
            _episodeFileRepository = episodeFileRepository;
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
                .Where(e => e.EpisodeFileId > 0)
                .Select(e => e.EpisodeFileId)
                .Distinct()
                .Select(id => _episodeFileRepository.Get(id))
                .Where(f => f != null)
                .ToList();

            result.OldFiles.AddRange(existingFiles);

            // Move or copy the file
            episodeFile = _episodeFileMover.MoveEpisodeFile(episodeFile, localEpisode);

            // Delete old files if not copying
            if (!copyOnly)
            {
                foreach (var file in existingFiles)
                {
                    _mediaFileService.Delete(file, DeleteMediaFileReason.Upgrade);
                }
            }

            result.EpisodeFile = episodeFile;
            return result;
        }

        // Book interface implementation (stub for now)
        public BookFileUpgradeResult UpgradeBookFile(BookFile bookFile, Parser.Model.LocalBook localBook, bool copyOnly = false)
        {
            // TODO: Implement book file upgrade
            throw new NotImplementedException();
        }
    }
}
