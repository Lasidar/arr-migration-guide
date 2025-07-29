using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Readarr.Common;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Tv;
using Readarr.Core.Tv.Events;
using System;

namespace Readarr.Core.MediaFiles
{
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

        public EpisodeFile Add(EpisodeFile episodeFile)
        {
            var addedFile = _mediaFileRepository.Insert(episodeFile);
            _eventAggregator.PublishEvent(new EpisodeFileAddedEvent(addedFile));
            return addedFile;
        }

        public void Update(EpisodeFile episodeFile)
        {
            _mediaFileRepository.Update(episodeFile);
        }

        public void Update(List<EpisodeFile> episodeFiles)
        {
            _mediaFileRepository.UpdateMany(episodeFiles);
        }

        public void Delete(EpisodeFile episodeFile, DeleteMediaFileReason reason)
        {
            // Little hack so we have the episodes and series attached for the event consumers
            episodeFile.Episodes.LazyLoad();
            episodeFile.Path = Path.Combine(episodeFile.Series.Value.Path, episodeFile.RelativePath);

            _mediaFileRepository.Delete(episodeFile);
            _eventAggregator.PublishEvent(new EpisodeFileDeletedEvent(episodeFile, reason));
        }

        public List<EpisodeFile> GetFilesBySeries(int seriesId)
        {
            return _mediaFileRepository.GetFilesBySeries(seriesId);
        }

        public List<EpisodeFile> GetFilesBySeriesIds(List<int> seriesIds)
        {
            return _mediaFileRepository.GetFilesBySeriesIds(seriesIds);
        }

        public List<EpisodeFile> GetFilesBySeason(int seriesId, int seasonNumber)
        {
            return _mediaFileRepository.GetFilesBySeason(seriesId, seasonNumber);
        }

        public List<EpisodeFile> GetFiles(IEnumerable<int> ids)
        {
            return _mediaFileRepository.Get(ids).ToList();
        }

        public List<EpisodeFile> GetFilesWithoutMediaInfo()
        {
            return _mediaFileRepository.GetFilesWithoutMediaInfo();
        }

        public List<string> FilterExistingFiles(List<string> files, Series series)
        {
            var seriesFiles = GetFilesBySeries(series.Id);

            return FilterExistingFiles(files, seriesFiles, series);
        }

        public EpisodeFile Get(int id)
        {
            return _mediaFileRepository.Get(id);
        }

        public List<EpisodeFile> Get(IEnumerable<int> ids)
        {
            return _mediaFileRepository.Get(ids).ToList();
        }

        public List<EpisodeFile> GetFilesWithRelativePath(int seriesId, string relativePath)
        {
            return _mediaFileRepository.GetFilesWithRelativePath(seriesId, relativePath);
        }

        public void HandleAsync(SeriesDeletedEvent message)
        {
            var files = GetFilesBySeries(message.Series.Id);
            _mediaFileRepository.DeleteMany(files);
        }

        // Book interface implementations (stubs for now)
        public void Delete(BookFile bookFile)
        {
            // TODO: Implement book file deletion
            throw new NotImplementedException();
        }

        BookFile IMediaFileService.Get(int id)
        {
            // TODO: Implement book file retrieval
            throw new NotImplementedException();
        }

        List<BookFile> IMediaFileService.Get(IEnumerable<int> ids)
        {
            // TODO: Implement book files retrieval
            throw new NotImplementedException();
        }

        public List<BookFile> GetFilesByAuthor(int authorId)
        {
            // TODO: Implement get files by author
            throw new NotImplementedException();
        }

        public List<BookFile> GetFilesByBook(int bookId)
        {
            // TODO: Implement get files by book
            throw new NotImplementedException();
        }

        public List<BookFile> GetUnmappedFiles()
        {
            // TODO: Implement get unmapped files
            throw new NotImplementedException();
        }

        public void UpdateMediaInfo(List<BookFile> bookFiles)
        {
            // TODO: Implement update media info
            throw new NotImplementedException();
        }

        public static List<string> FilterExistingFiles(List<string> files, List<EpisodeFile> seriesFiles, Series series)
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
