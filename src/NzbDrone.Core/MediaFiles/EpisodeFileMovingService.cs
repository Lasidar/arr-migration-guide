using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.EnsureThat;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.MediaFiles.EpisodeImport;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.RootFolders;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.MediaFiles
{
    public interface IMoveEditionFiles
    {
        EditionFile MoveEditionFile(EditionFile episodeFile, Series series);
        EditionFile MoveEditionFile(EditionFile episodeFile, LocalEpisode localEpisode);
        EditionFile CopyEditionFile(EditionFile episodeFile, LocalEpisode localEpisode);
    }

    public class EditionFileMovingService : IMoveEditionFiles
    {
        private readonly IEditionService _episodeService;
        private readonly IUpdateEditionFileService _updateEditionFileService;
        private readonly IBuildFileNames _buildFileNames;
        private readonly IDiskTransferService _diskTransferService;
        private readonly IDiskProvider _diskProvider;
        private readonly IMediaFileAttributeService _mediaFileAttributeService;
        private readonly IImportScript _scriptImportDecider;
        private readonly IRootFolderService _rootFolderService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IConfigService _configService;
        private readonly Logger _logger;

        public EditionFileMovingService(IEditionService episodeService,
                                IUpdateEditionFileService updateEditionFileService,
                                IBuildFileNames buildFileNames,
                                IDiskTransferService diskTransferService,
                                IDiskProvider diskProvider,
                                IMediaFileAttributeService mediaFileAttributeService,
                                IImportScript scriptImportDecider,
                                IRootFolderService rootFolderService,
                                IEventAggregator eventAggregator,
                                IConfigService configService,
                                Logger logger)
        {
            _episodeService = episodeService;
            _updateEditionFileService = updateEditionFileService;
            _buildFileNames = buildFileNames;
            _diskTransferService = diskTransferService;
            _diskProvider = diskProvider;
            _mediaFileAttributeService = mediaFileAttributeService;
            _scriptImportDecider = scriptImportDecider;
            _rootFolderService = rootFolderService;
            _eventAggregator = eventAggregator;
            _configService = configService;
            _logger = logger;
        }

        public EditionFile MoveEditionFile(EditionFile episodeFile, Series series)
        {
            var episodes = _episodeService.GetEpisodesByFileId(episodeFile.Id);
            return MoveEditionFile(episodeFile, series, episodes);
        }

        private EditionFile MoveEditionFile(EditionFile episodeFile, Series series, List<Episode> episodes)
        {
            var filePath = _buildFileNames.BuildFilePath(episodes, series, episodeFile, Path.GetExtension(episodeFile.RelativePath));

            EnsureEpisodeFolder(episodeFile, series, episodes.Select(v => v.BookNumber).First(), filePath);

            _logger.Debug("Renaming episode file: {0} to {1}", episodeFile, filePath);

            return TransferFile(episodeFile, series, episodes, filePath, TransferMode.Move);
        }

        public EditionFile MoveEditionFile(EditionFile episodeFile, LocalEpisode localEpisode)
        {
            var filePath = _buildFileNames.BuildFilePath(localEpisode.Episodes, localEpisode.Series, episodeFile, Path.GetExtension(localEpisode.Path), null, localEpisode.CustomFormats);

            EnsureEpisodeFolder(episodeFile, localEpisode, filePath);

            _logger.Debug("Moving episode file: {0} to {1}", episodeFile.Path, filePath);

            return TransferFile(episodeFile, localEpisode.Series, localEpisode.Episodes, filePath, TransferMode.Move, localEpisode);
        }

        public EditionFile CopyEditionFile(EditionFile episodeFile, LocalEpisode localEpisode)
        {
            var filePath = _buildFileNames.BuildFilePath(localEpisode.Episodes, localEpisode.Series, episodeFile, Path.GetExtension(localEpisode.Path), null, localEpisode.CustomFormats);

            EnsureEpisodeFolder(episodeFile, localEpisode, filePath);

            if (_configService.CopyUsingHardlinks)
            {
                _logger.Debug("Attempting to hardlink episode file: {0} to {1}", episodeFile.Path, filePath);
                return TransferFile(episodeFile, localEpisode.Series, localEpisode.Episodes, filePath, TransferMode.HardLinkOrCopy, localEpisode);
            }

            _logger.Debug("Copying episode file: {0} to {1}", episodeFile.Path, filePath);
            return TransferFile(episodeFile, localEpisode.Series, localEpisode.Episodes, filePath, TransferMode.Copy, localEpisode);
        }

        private EditionFile TransferFile(EditionFile episodeFile, Series series, List<Episode> episodes, string destinationFilePath, TransferMode mode, LocalEpisode localEpisode = null)
        {
            Ensure.That(episodeFile, () => episodeFile).IsNotNull();
            Ensure.That(series, () => series).IsNotNull();
            Ensure.That(destinationFilePath, () => destinationFilePath).IsValidPath(PathValidationType.CurrentOs);

            var episodeFilePath = episodeFile.Path ?? Path.Combine(series.Path, episodeFile.RelativePath);

            if (!_diskProvider.FileExists(episodeFilePath))
            {
                throw new FileNotFoundException("Episode file path does not exist", episodeFilePath);
            }

            if (episodeFilePath == destinationFilePath)
            {
                throw new SameFilenameException("File not moved, source and destination are the same", episodeFilePath);
            }

            episodeFile.RelativePath = series.Path.GetRelativePath(destinationFilePath);

            if (localEpisode is not null)
            {
                localEpisode.FileNameBeforeRename = episodeFile.RelativePath;
            }

            if (localEpisode is not null && _scriptImportDecider.TryImport(episodeFilePath, destinationFilePath, localEpisode, episodeFile, mode) is var scriptImportDecision && scriptImportDecision != ScriptImportDecision.DeferMove)
            {
                if (scriptImportDecision == ScriptImportDecision.RenameRequested)
                {
                    try
                    {
                        MoveEditionFile(episodeFile, series, episodeFile.Episodes);
                    }
                    catch (SameFilenameException)
                    {
                        _logger.Debug("No rename was required. File already exists at destination.");
                    }
                }
            }
            else
            {
                _diskTransferService.TransferFile(episodeFilePath, destinationFilePath, mode);
            }

            _updateEditionFileService.ChangeFileDateForFile(episodeFile, series, episodes);

            try
            {
                _mediaFileAttributeService.SetFolderLastWriteTime(series.Path, episodeFile.DateAdded);

                if (series.SeasonFolder)
                {
                    var seasonFolder = Path.GetDirectoryName(destinationFilePath);

                    _mediaFileAttributeService.SetFolderLastWriteTime(seasonFolder, episodeFile.DateAdded);
                }
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Unable to set last write time");
            }

            _mediaFileAttributeService.SetFilePermissions(destinationFilePath);

            return episodeFile;
        }

        private void EnsureEpisodeFolder(EditionFile episodeFile, LocalEpisode localEpisode, string filePath)
        {
            EnsureEpisodeFolder(episodeFile, localEpisode.Series, localEpisode.BookNumber, filePath);
        }

        private void EnsureEpisodeFolder(EditionFile episodeFile, Series series, int seasonNumber, string filePath)
        {
            var episodeFolder = Path.GetDirectoryName(filePath);
            var seasonFolder = _buildFileNames.BuildSeasonPath(series, seasonNumber);
            var seriesFolder = series.Path;
            var rootFolder = _rootFolderService.GetBestRootFolderPath(seriesFolder);

            if (rootFolder.IsNullOrWhiteSpace())
            {
                throw new RootFolderNotFoundException($"Root folder was not found, '{seriesFolder}' is not a subdirectory of a defined root folder.");
            }

            if (!_diskProvider.FolderExists(rootFolder))
            {
                throw new RootFolderNotFoundException($"Root folder '{rootFolder}' was not found.");
            }

            var changed = false;
            var newEvent = new EpisodeFolderCreatedEvent(series, episodeFile);

            if (!_diskProvider.FolderExists(seriesFolder))
            {
                CreateFolder(seriesFolder);
                newEvent.SeriesFolder = seriesFolder;
                changed = true;
            }

            if (seriesFolder != seasonFolder && !_diskProvider.FolderExists(seasonFolder))
            {
                CreateFolder(seasonFolder);
                newEvent.SeasonFolder = seasonFolder;
                changed = true;
            }

            if (seasonFolder != episodeFolder && !_diskProvider.FolderExists(episodeFolder))
            {
                CreateFolder(episodeFolder);
                newEvent.EpisodeFolder = episodeFolder;
                changed = true;
            }

            if (changed)
            {
                _eventAggregator.PublishEvent(newEvent);
            }
        }

        private void CreateFolder(string directoryName)
        {
            Ensure.That(directoryName, () => directoryName).IsNotNullOrWhiteSpace();

            var parentFolder = new OsPath(directoryName).Directory.FullPath;
            if (!_diskProvider.FolderExists(parentFolder))
            {
                CreateFolder(parentFolder);
            }

            try
            {
                _diskProvider.CreateFolder(directoryName);
            }
            catch (IOException ex)
            {
                _logger.Error(ex, "Unable to create directory: {0}", directoryName);
            }

            _mediaFileAttributeService.SetFolderPermissions(directoryName);
        }
    }
}
