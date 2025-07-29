using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books;
using NzbDrone.Core.Books.Events;

namespace NzbDrone.Core.Extras.Files
{
    public interface IExtraFileService<TExtraFile>
        where TExtraFile : ExtraFile, new()
    {
        List<TExtraFile> GetFilesBySeries(int seriesId);
        List<TExtraFile> GetFilesByEditionFile(int episodeFileId);
        TExtraFile FindByPath(int seriesId, string path);
        void Upsert(TExtraFile extraFile);
        void Upsert(List<TExtraFile> extraFiles);
        void Delete(int id);
        void DeleteMany(IEnumerable<int> ids);
    }

    public abstract class ExtraFileService<TExtraFile> : IExtraFileService<TExtraFile>,
                                                         IHandleAsync<SeriesDeletedEvent>,
                                                         IHandle<EditionFileDeletedEvent>
        where TExtraFile : ExtraFile, new()
    {
        private readonly IExtraFileRepository<TExtraFile> _repository;
        private readonly IAuthorService _seriesService;
        private readonly IDiskProvider _diskProvider;
        private readonly IRecycleBinProvider _recycleBinProvider;
        private readonly Logger _logger;

        public ExtraFileService(IExtraFileRepository<TExtraFile> repository,
                                IAuthorService seriesService,
                                IDiskProvider diskProvider,
                                IRecycleBinProvider recycleBinProvider,
                                Logger logger)
        {
            _repository = repository;
            _seriesService = seriesService;
            _diskProvider = diskProvider;
            _recycleBinProvider = recycleBinProvider;
            _logger = logger;
        }

        public List<TExtraFile> GetFilesBySeries(int seriesId)
        {
            return _repository.GetFilesBySeries(seriesId);
        }

        public List<TExtraFile> GetFilesByEditionFile(int episodeFileId)
        {
            return _repository.GetFilesByEditionFile(episodeFileId);
        }

        public TExtraFile FindByPath(int seriesId, string path)
        {
            return _repository.FindByPath(seriesId, path);
        }

        public void Upsert(TExtraFile extraFile)
        {
            Upsert(new List<TExtraFile> { extraFile });
        }

        public void Upsert(List<TExtraFile> extraFiles)
        {
            extraFiles.ForEach(m =>
            {
                m.LastUpdated = DateTime.UtcNow;

                if (m.Id == 0)
                {
                    m.Added = m.LastUpdated;
                }
            });

            _repository.InsertMany(extraFiles.Where(m => m.Id == 0).ToList());
            _repository.UpdateMany(extraFiles.Where(m => m.Id > 0).ToList());
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public void DeleteMany(IEnumerable<int> ids)
        {
            _repository.DeleteMany(ids);
        }

        public void HandleAsync(SeriesDeletedEvent message)
        {
            _logger.Debug("Deleting Extra from database for series: {0}", string.Join(',', message.Series));
            _repository.DeleteForAuthorIds(message.Series.Select(m => m.Id).ToList());
        }

        public void Handle(EditionFileDeletedEvent message)
        {
            var episodeFile = message.EditionFile;

            if (message.Reason == DeleteMediaFileReason.NoLinkedEpisodes)
            {
                _logger.Debug("Removing episode file from DB as part of cleanup routine, not deleting extra files from disk.");
            }
            else
            {
                var series = _seriesService.GetSeries(message.EditionFile.AuthorId);

                foreach (var extra in _repository.GetFilesByEditionFile(episodeFile.Id))
                {
                    var path = Path.Combine(series.Path, extra.RelativePath);

                    if (_diskProvider.FileExists(path))
                    {
                        // Send to the recycling bin so they can be recovered if necessary
                        var subfolder = _diskProvider.GetParentFolder(series.Path).GetRelativePath(_diskProvider.GetParentFolder(path));
                        _recycleBinProvider.DeleteFile(path, subfolder);
                    }
                }
            }

            _logger.Debug("Deleting Extra from database for episode file: {0}", episodeFile);
            _repository.DeleteForEditionFile(episodeFile.Id);
        }
    }
}
