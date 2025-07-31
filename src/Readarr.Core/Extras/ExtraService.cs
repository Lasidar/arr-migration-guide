using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Readarr.Common.Disk;
using Readarr.Core.Configuration;
using Readarr.Core.Download;
using Readarr.Core.Extras.Files;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser.Model;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Extras
{
    public interface IExtraService
    {
        void MoveFilesAfterRename(Tv.Series series, EpisodeFile episodeFile);
        void ImportEpisode(LocalEpisode localEpisode, EpisodeFile episodeFile, bool isReadOnly);
    }

    public class ExtraService : IExtraService,
                                IHandle<MediaCoversUpdatedEvent>,
                                IHandle<EpisodeFolderCreatedEvent>,
                                IHandle<SeriesScannedEvent>,
                                IHandle<AuthorRenamedEvent>,
                                IHandle<DownloadsProcessedEvent>
    {
        private readonly MediaFiles.IMediaFileService _mediaFileService;
        private readonly IEpisodeService _episodeService;
        private readonly IDiskProvider _diskProvider;
        private readonly IConfigService _configService;
        private readonly List<IManageExtraFiles> _extraFileManagers;
        private readonly Dictionary<int, Series> _seriesWithImportedFiles;

        public ExtraService(MediaFiles.IMediaFileService mediaFileService,
                            IEpisodeService episodeService,
                            IDiskProvider diskProvider,
                            IConfigService configService,
                            IEnumerable<IManageExtraFiles> extraFileManagers,
                            Logger logger)
        {
            _mediaFileService = mediaFileService;
            _episodeService = episodeService;
            _diskProvider = diskProvider;
            _configService = configService;
            _extraFileManagers = extraFileManagers.OrderBy(e => e.Order).ToList();
            _seriesWithImportedFiles = new Dictionary<int, Series>();
        }

        public void ImportEpisode(LocalEpisode localEpisode, EpisodeFile episodeFile, bool isReadOnly)
        {
            ImportExtraFiles(localEpisode, episodeFile, isReadOnly);

            CreateAfterEpisodeImport(localEpisode.Series, episodeFile);
        }

        private void ImportExtraFiles(LocalEpisode localEpisode, EpisodeFile episodeFile, bool isReadOnly)
        {
            if (!_configService.ImportExtraFiles)
            {
                return;
            }

            var folderSearchOption = localEpisode.FolderEpisodeInfo != null;

            var wantedExtensions = _configService.ExtraFileExtensions.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                                                     .Select(e => e.Trim(' ', '.')
                                                                     .Insert(0, "."))
                                                                     .ToList();

            var sourceFolder = _diskProvider.GetParentFolder(localEpisode.Path);
            var files = _diskProvider.GetFiles(sourceFolder, folderSearchOption);
            var managedFiles = _extraFileManagers.Select((i) => new List<string>()).ToArray();

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file);
                var matchingExtension = wantedExtensions.FirstOrDefault(e => e.Equals(extension));

                if (matchingExtension == null)
                {
                    continue;
                }

                for (var i = 0; i < _extraFileManagers.Count; i++)
                {
                    if (_extraFileManagers[i].CanImportFile(localEpisode, episodeFile, file, extension, isReadOnly))
                    {
                        managedFiles[i].Add(file);
                        break;
                    }
                }
            }

            for (var i = 0; i < _extraFileManagers.Count; i++)
            {
                _extraFileManagers[i].ImportFiles(localEpisode, episodeFile, managedFiles[i], isReadOnly);
            }
        }

        private void CreateAfterEpisodeImport(Tv.Series series, EpisodeFile episodeFile)
        {
            lock (_seriesWithImportedFiles)
            {
                _seriesWithImportedFiles.TryAdd(series.Id, series);
            }

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.CreateAfterEpisodeImport(series, episodeFile);
            }
        }

        public void Handle(MediaCoversUpdatedEvent message)
        {
            if (message.Updated)
            {
                var series = message.Series;

                foreach (var extraFileManager in _extraFileManagers)
                {
                    extraFileManager.CreateAfterMediaCoverUpdate(series);
                }
            }
        }

        public void Handle(SeriesScannedEvent message)
        {
            var series = message.Series;
            var episodeFiles = GetEpisodeFiles(series.Id);

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.CreateAfterSeriesScan(series, episodeFiles);
            }
        }

        public void Handle(EpisodeFolderCreatedEvent message)
        {
            var series = message.Series;

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.CreateAfterEpisodeFolder(series, message.SeriesFolder, message.SeasonFolder);
            }
        }

        public void MoveFilesAfterRename(Tv.Series series, EpisodeFile episodeFile)
        {
            var episodeFiles = new List<EpisodeFile> { episodeFile };

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.MoveFilesAfterRename(series, episodeFiles);
            }
        }

        public void Handle(AuthorRenamedEvent message)
        {
            var series = message.Series;
            var episodeFiles = GetEpisodeFiles(series.Id);

            foreach (var extraFileManager in _extraFileManagers)
            {
                extraFileManager.MoveFilesAfterRename(series, episodeFiles);
            }
        }

        public void Handle(DownloadsProcessedEvent message)
        {
            var allSeries = new List<Tv.Series>();

            lock (_seriesWithImportedFiles)
            {
                allSeries.AddRange(_seriesWithImportedFiles.Values);

                _seriesWithImportedFiles.Clear();
            }

            foreach (var series in allSeries)
            {
                foreach (var extraFileManager in _extraFileManagers)
                {
                    extraFileManager.CreateAfterEpisodesImported(series);
                }
            }
        }

        private List<EpisodeFile> GetEpisodeFiles(int seriesId)
        {
            var episodeFiles = _mediaFileService.GetFilesBySeries(seriesId);
            var episodes = _episodeService.GetEpisodeBySeries(seriesId);

            foreach (var episodeFile in episodeFiles)
            {
                var localEpisodeFile = episodeFile;
                episodeFile.Episodes = new List<Episode>(episodes.Where(e => e.EpisodeFileId == localEpisodeFile.Id));
            }

            return episodeFiles;
        }
    }
}
