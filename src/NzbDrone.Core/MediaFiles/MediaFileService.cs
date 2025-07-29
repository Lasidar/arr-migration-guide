using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using NzbDrone.Common;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books;
using NzbDrone.Core.Books.Events;

namespace NzbDrone.Core.MediaFiles
{
    public interface IMediaFileService
    {
        EditionFile Add(EditionFile episodeFile);
        void Update(EditionFile episodeFile);
        void Update(List<EditionFile> episodeFiles);
        void Delete(EditionFile episodeFile, DeleteMediaFileReason reason);
        List<EditionFile> GetFilesBySeries(int seriesId);
        List<EditionFile> GetFilesByAuthorIds(List<int> seriesIds);
        List<EditionFile> GetFilesBySeason(int seriesId, int seasonNumber);
        List<EditionFile> GetFiles(IEnumerable<int> ids);
        List<EditionFile> GetFilesWithoutMediaInfo();
        List<string> FilterExistingFiles(List<string> files, Series series);
        EditionFile Get(int id);
        List<EditionFile> Get(IEnumerable<int> ids);
        List<EditionFile> GetFilesWithRelativePath(int seriesId, string relativePath);
    }

    public class MediaFileService : IMediaFileService, IHandleAsync<SeriesDeletedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMediaFileRepository _mediaFileRepository;
        private readonly Logger _logger;

        public MediaFileService(IMediaFileRepository mediaFileRepository, IEventAggregator eventAggregator, Logger logger)
        {
            _mediaFileRepository = mediaFileRepository;
            _eventAggregator = eventAggregator;
            _logger = logger;
        }

        public EditionFile Add(EditionFile episodeFile)
        {
            var addedFile = _mediaFileRepository.Insert(episodeFile);
            _eventAggregator.PublishEvent(new EditionFileAddedEvent(addedFile));
            return addedFile;
        }

        public void Update(EditionFile episodeFile)
        {
            _mediaFileRepository.Update(episodeFile);
        }

        public void Update(List<EditionFile> episodeFiles)
        {
            _mediaFileRepository.UpdateMany(episodeFiles);
        }

        public void Delete(EditionFile episodeFile, DeleteMediaFileReason reason)
        {
            // Little hack so we have the episodes and series attached for the event consumers
            episodeFile.Episodes.LazyLoad();
            episodeFile.Path = Path.Combine(episodeFile.Series.Value.Path, episodeFile.RelativePath);

            _mediaFileRepository.Delete(episodeFile);
            _eventAggregator.PublishEvent(new EditionFileDeletedEvent(episodeFile, reason));
        }

        public List<EditionFile> GetFilesBySeries(int seriesId)
        {
            return _mediaFileRepository.GetFilesBySeries(seriesId);
        }

        public List<EditionFile> GetFilesByAuthorIds(List<int> seriesIds)
        {
            return _mediaFileRepository.GetFilesByAuthorIds(seriesIds);
        }

        public List<EditionFile> GetFilesBySeason(int seriesId, int seasonNumber)
        {
            return _mediaFileRepository.GetFilesBySeason(seriesId, seasonNumber);
        }

        public List<EditionFile> GetFiles(IEnumerable<int> ids)
        {
            return _mediaFileRepository.Get(ids).ToList();
        }

        public List<EditionFile> GetFilesWithoutMediaInfo()
        {
            return _mediaFileRepository.GetFilesWithoutMediaInfo();
        }

        public List<string> FilterExistingFiles(List<string> files, Series series)
        {
            var seriesFiles = GetFilesBySeries(series.Id);

            return FilterExistingFiles(files, seriesFiles, series);
        }

        public EditionFile Get(int id)
        {
            return _mediaFileRepository.Get(id);
        }

        public List<EditionFile> Get(IEnumerable<int> ids)
        {
            return _mediaFileRepository.Get(ids).ToList();
        }

        public List<EditionFile> GetFilesWithRelativePath(int seriesId, string relativePath)
        {
            return _mediaFileRepository.GetFilesWithRelativePath(seriesId, relativePath);
        }

        public void HandleAsync(SeriesDeletedEvent message)
        {
            _mediaFileRepository.DeleteForSeries(message.Series.Select(s => s.Id).ToList());
        }

        public static List<string> FilterExistingFiles(List<string> files, List<EditionFile> seriesFiles, Series series)
        {
            var seriesFilePaths = seriesFiles.Select(f => Path.Combine(series.Path, f.RelativePath)).ToList();

            if (!seriesFilePaths.Any())
            {
                return files;
            }

            return files.Except(seriesFilePaths, PathEqualityComparer.Instance).ToList();
        }
    }
}
