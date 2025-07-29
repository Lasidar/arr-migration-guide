using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Extras.Files;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Extras.Others
{
    public class OtherExtraService : ExtraFileManager<OtherExtraFile>
    {
        private readonly IDiskProvider _diskProvider;
        private readonly IOtherExtraFileService _otherExtraFileService;
        private readonly IMediaFileAttributeService _mediaFileAttributeService;
        private readonly Logger _logger;

        public OtherExtraService(IConfigService configService,
                                 IDiskProvider diskProvider,
                                 IDiskTransferService diskTransferService,
                                 IOtherExtraFileService otherExtraFileService,
                                 IMediaFileAttributeService mediaFileAttributeService,
                                 Logger logger)
            : base(configService, diskProvider, diskTransferService, logger)
        {
            _diskProvider = diskProvider;
            _otherExtraFileService = otherExtraFileService;
            _mediaFileAttributeService = mediaFileAttributeService;
            _logger = logger;
        }

        public override int Order => 2;

        public override IEnumerable<ExtraFile> CreateAfterMediaCoverUpdate(Series series)
        {
            return Enumerable.Empty<ExtraFile>();
        }

        public override IEnumerable<ExtraFile> CreateAfterSeriesScan(Series series, List<EditionFile> episodeFiles)
        {
            return Enumerable.Empty<ExtraFile>();
        }

        public override IEnumerable<ExtraFile> CreateAfterEpisodesImported(Series series)
        {
            return Enumerable.Empty<ExtraFile>();
        }

        public override IEnumerable<ExtraFile> CreateAfterEpisodeImport(Series series, EditionFile episodeFile)
        {
            return Enumerable.Empty<ExtraFile>();
        }

        public override IEnumerable<ExtraFile> CreateAfterEpisodeFolder(Series series, string seriesFolder, string seasonFolder)
        {
            return Enumerable.Empty<ExtraFile>();
        }

        public override IEnumerable<ExtraFile> MoveFilesAfterRename(Series series, List<EditionFile> episodeFiles)
        {
            var extraFiles = _otherExtraFileService.GetFilesBySeries(series.Id);
            var movedFiles = new List<OtherExtraFile>();

            foreach (var episodeFile in episodeFiles)
            {
                var extraFilesForEditionFile = extraFiles.Where(m => m.EditionFileId == episodeFile.Id).ToList();

                foreach (var extraFile in extraFilesForEditionFile)
                {
                    movedFiles.AddIfNotNull(MoveFile(series, episodeFile, extraFile));
                }
            }

            _otherExtraFileService.Upsert(movedFiles);

            return movedFiles;
        }

        public override bool CanImportFile(LocalEpisode localEpisode, EditionFile episodeFile, string path, string extension, bool readOnly)
        {
            return true;
        }

        public override IEnumerable<ExtraFile> ImportFiles(LocalEpisode localEpisode, EditionFile episodeFile, List<string> files, bool isReadOnly)
        {
            var importedFiles = new List<ExtraFile>();
            var filteredFiles = files.Where(f => CanImportFile(localEpisode, episodeFile, f, Path.GetExtension(f), isReadOnly)).ToList();
            var sourcePath = localEpisode.Path;
            var sourceFolder = _diskProvider.GetParentFolder(sourcePath);
            var sourceFileName = Path.GetFileNameWithoutExtension(sourcePath);
            var matchingFiles = new List<string>();
            var hasNfo = false;

            foreach (var file in filteredFiles)
            {
                try
                {
                    // Filter out duplicate NFO files
                    if (file.EndsWith(".nfo", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (hasNfo)
                        {
                            continue;
                        }

                        hasNfo = true;
                    }

                    // Filename match
                    if (Path.GetFileNameWithoutExtension(file).StartsWith(sourceFileName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        matchingFiles.Add(file);
                        continue;
                    }

                    // Season and episode match
                    var fileEpisodeInfo = Parser.Parser.ParsePath(file) ?? new ParsedEpisodeInfo();

                    if (fileEpisodeInfo.EditionNumbers.Length == 0)
                    {
                        continue;
                    }

                    if (fileEpisodeInfo.BookNumber == localEpisode.FileEpisodeInfo.BookNumber &&
                        fileEpisodeInfo.EditionNumbers.SequenceEqual(localEpisode.FileEpisodeInfo.EditionNumbers))
                    {
                        matchingFiles.Add(file);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to import extra file: {0}", file);
                }
            }

            foreach (var file in matchingFiles)
            {
                try
                {
                    var extraFile = ImportFile(localEpisode.Series, episodeFile, file, isReadOnly, Path.GetExtension(file), null);
                    _mediaFileAttributeService.SetFilePermissions(file);
                    _otherExtraFileService.Upsert(extraFile);
                    importedFiles.Add(extraFile);
                }
                catch (Exception ex)
                {
                    _logger.Warn(ex, "Failed to import extra file: {0}", file);
                }
            }

            return importedFiles;
        }
    }
}
