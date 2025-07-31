using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Books.Events;
using Readarr.Core.Datastore;
using Readarr.Core.Download;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;

namespace Readarr.Core.History
{
    public interface IBookHistoryService
    {
        PagingSpec<BookHistory> Paged(PagingSpec<BookHistory> pagingSpec, int[] languages, int[] qualities);
        BookHistory MostRecentForBook(int bookId);
        List<BookHistory> FindByBookId(int bookId);
        BookHistory MostRecentForDownloadId(string downloadId);
        BookHistory Get(int historyId);
        List<BookHistory> GetByAuthor(int authorId, HistoryEventType? eventType);
        List<BookHistory> GetByBook(int bookId, HistoryEventType? eventType);
        List<BookHistory> Find(string downloadId, HistoryEventType eventType);
        List<BookHistory> FindByDownloadId(string downloadId);
        string FindDownloadId(BookImportedEvent trackedDownload);
        List<BookHistory> Since(DateTime date, HistoryEventType? eventType);
    }

    public class BookHistoryService : IBookHistoryService,
                                      IHandle<BookGrabbedEvent>,
                                      IHandle<BookImportedEvent>,
                                      IHandle<DownloadFailedEvent>,
                                      IHandle<BookFileDeletedEvent>,
                                      IHandle<BookFileRenamedEvent>,
                                      IHandle<AuthorDeletedEvent>,
                                      IHandle<DownloadIgnoredEvent>
    {
        private readonly IHistoryRepository _historyRepository;
        private readonly Logger _logger;

        public BookHistoryService(IHistoryRepository historyRepository, Logger logger)
        {
            _historyRepository = historyRepository;
            _logger = logger;
        }

        public PagingSpec<BookHistory> Paged(PagingSpec<BookHistory> pagingSpec, int[] languages, int[] qualities)
        {
            return _historyRepository.GetPaged(pagingSpec, languages, qualities);
        }

        public BookHistory MostRecentForBook(int bookId)
        {
            return _historyRepository.MostRecentForBook(bookId);
        }

        public List<BookHistory> FindByBookId(int bookId)
        {
            return _historyRepository.GetByBook(bookId, null);
        }

        public BookHistory MostRecentForDownloadId(string downloadId)
        {
            return _historyRepository.MostRecentForDownloadId(downloadId);
        }

        public BookHistory Get(int historyId)
        {
            return _historyRepository.Get(historyId);
        }

        public List<BookHistory> GetByAuthor(int authorId, HistoryEventType? eventType)
        {
            return _historyRepository.GetByAuthor(authorId, eventType);
        }

        public List<BookHistory> GetByBook(int bookId, HistoryEventType? eventType)
        {
            return _historyRepository.GetByBook(bookId, eventType);
        }

        public List<BookHistory> Find(string downloadId, HistoryEventType eventType)
        {
            return _historyRepository.FindByDownloadId(downloadId).Where(h => h.EventType == eventType).ToList();
        }

        public List<BookHistory> FindByDownloadId(string downloadId)
        {
            return _historyRepository.FindByDownloadId(downloadId);
        }

        public string FindDownloadId(BookImportedEvent trackedDownload)
        {
            var bookHistory = _historyRepository.MostRecentForBook(trackedDownload.BookInfo.Book.Id);

            if (bookHistory == null || bookHistory.EventType != HistoryEventType.Grabbed)
            {
                return null;
            }

            var downloadHistory = _historyRepository.FindByDownloadId(bookHistory.DownloadId)
                .Where(c => c.EventType != HistoryEventType.Grabbed && c.DownloadId != null)
                .ToList();

            var stillDownloading = downloadHistory.Where(c => c.EventType == HistoryEventType.Grabbed && !downloadHistory.Any(d => d.BookId == c.BookId)).ToList();

            string downloadId = null;

            if (stillDownloading.Any())
            {
                foreach (var bookId in trackedDownload.BookInfo.Books.Select(c => c.Id))
                {
                    var book = stillDownloading.SingleOrDefault(c => c.BookId == bookId);

                    if (book != null)
                    {
                        downloadId = book.DownloadId;
                    }
                }
            }

            return downloadId;
        }

        public List<BookHistory> Since(DateTime date, HistoryEventType? eventType)
        {
            return _historyRepository.Since(date, eventType);
        }

        public void Handle(BookGrabbedEvent message)
        {
            var history = new BookHistory
            {
                EventType = HistoryEventType.Grabbed,
                Date = DateTime.UtcNow,
                Quality = message.Book.ParsedBookInfo.Quality,
                Languages = message.Book.ParsedBookInfo.Languages,
                SourceTitle = message.Book.Release.Title,
                AuthorId = message.Book.Author.Id,
                BookId = message.Book.Books.First().Id,
                DownloadId = message.DownloadId,
                Data = new Dictionary<string, string>()
            };

            history.Data.Add("Indexer", message.Book.Release.Indexer ?? string.Empty);
            history.Data.Add("ReadarrClient", message.DownloadClient ?? string.Empty);
            history.Data.Add("DownloadProtocol", ((int)message.Book.Release.DownloadProtocol).ToString());
            history.Data.Add("CustomFormatScore", message.Book.CustomFormatScore.ToString());

            if (!message.Book.ParsedBookInfo.ReleaseHash.IsNullOrWhiteSpace())
            {
                history.Data.Add("ReleaseHash", message.Book.ParsedBookInfo.ReleaseHash);
            }

            _historyRepository.Insert(history);
        }

        public void Handle(BookImportedEvent message)
        {
            if (!message.NewDownload)
            {
                return;
            }

            var history = new BookHistory
            {
                EventType = HistoryEventType.DownloadFolderImported,
                Date = DateTime.UtcNow,
                Quality = message.BookInfo.Quality,
                Languages = message.BookInfo.Languages,
                SourceTitle = message.ImportedBook.SceneName ?? Path.GetFileNameWithoutExtension(message.BookInfo.Path),
                AuthorId = message.BookInfo.Author.Id,
                BookId = message.BookInfo.Book.Id,
                DownloadId = message.DownloadId,
                Data = new Dictionary<string, string>()
            };

            history.Data.Add("FileId", message.ImportedBook.Id.ToString());
            history.Data.Add("DroppedPath", message.BookInfo.Path);
            history.Data.Add("ImportedPath", message.ImportedBook.Path);
            history.Data.Add("DownloadClient", message.DownloadClientInfo?.Type ?? string.Empty);
            history.Data.Add("DownloadClientName", message.DownloadClientInfo?.Name ?? string.Empty);

            if (message.BookInfo.ReleaseGroup.IsNotNullOrWhiteSpace())
            {
                history.Data.Add("ReleaseGroup", message.BookInfo.ReleaseGroup);
            }

            _historyRepository.Insert(history);
        }

        public void Handle(DownloadFailedEvent message)
        {
            var history = new BookHistory
            {
                EventType = HistoryEventType.DownloadFailed,
                Date = DateTime.UtcNow,
                Quality = message.Quality,
                Languages = message.Languages,
                SourceTitle = message.SourceTitle,
                AuthorId = message.AuthorId,
                BookId = message.BookIds.FirstOrDefault(),
                DownloadId = message.DownloadId,
                Data = new Dictionary<string, string>()
            };

            history.Data.Add("DownloadClient", message.DownloadClient ?? string.Empty);
            history.Data.Add("DownloadClientName", message.TrackedDownload?.DownloadItem.DownloadClientInfo.Name ?? string.Empty);
            history.Data.Add("Message", message.Message);

            _historyRepository.Insert(history);
        }

        public void Handle(BookFileDeletedEvent message)
        {
            if (message.Reason == DeleteMediaFileReason.NoLinkedEpisodes)
            {
                _logger.Debug("Removing book file from DB as part of cleanup routine, not creating history event.");
                return;
            }

            var history = new BookHistory
            {
                EventType = HistoryEventType.BookFileDeleted,
                Date = DateTime.UtcNow,
                Quality = message.BookFile.Quality,
                Languages = message.BookFile.Languages,
                SourceTitle = message.BookFile.Path,
                AuthorId = message.BookFile.Author.Value.Id,
                BookId = message.BookFile.BookId,
                Data = new Dictionary<string, string>()
            };

            history.Data.Add("Reason", message.Reason.ToString());

            _historyRepository.Insert(history);
        }

        public void Handle(BookFileRenamedEvent message)
        {
            var sourcePath = message.OriginalPath;
            var path = message.BookFile.Path;

            var history = new BookHistory
            {
                EventType = HistoryEventType.BookFileRenamed,
                Date = DateTime.UtcNow,
                Quality = message.BookFile.Quality,
                Languages = message.BookFile.Languages,
                SourceTitle = message.OriginalPath,
                AuthorId = message.BookFile.Author.Value.Id,
                BookId = message.BookFile.BookId,
                Data = new Dictionary<string, string>()
            };

            history.Data.Add("SourcePath", sourcePath);
            history.Data.Add("Path", path);

            _historyRepository.Insert(history);
        }

        public void Handle(AuthorDeletedEvent message)
        {
            _historyRepository.DeleteForAuthor(message.Author.Select(a => a.Id).ToList());
        }

        public void Handle(DownloadIgnoredEvent message)
        {
            var history = new BookHistory
            {
                EventType = HistoryEventType.DownloadIgnored,
                Date = DateTime.UtcNow,
                Quality = message.Quality,
                Languages = message.Languages,
                SourceTitle = message.SourceTitle,
                AuthorId = message.AuthorId,
                BookId = message.BookIds.FirstOrDefault(),
                DownloadId = message.DownloadId,
                Data = new Dictionary<string, string>()
            };

            history.Data.Add("DownloadClient", message.DownloadClientInfo.Name ?? string.Empty);
            history.Data.Add("Message", message.Message);

            _historyRepository.Insert(history);
        }
    }
}