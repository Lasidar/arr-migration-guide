using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog;
using Readarr.Common.Disk;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Books.Events;
using Readarr.Core.Download;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Qualities;

namespace Readarr.Core.MediaFiles.BookImport
{
    public interface IBookImportService
    {
        List<ImportResult> Import(List<ImportDecision<LocalBook>> decisions, bool newDownload, DownloadClientItem downloadClientItem = null, ImportMode importMode = ImportMode.Auto);
    }

    public class BookImportService : IBookImportService
    {
        private readonly IBookFileService _bookFileService;
        private readonly IBookService _bookService;
        private readonly IEditionService _editionService;
        private readonly IUpgradeMediaFiles _bookFileUpgrader;
        private readonly IMediaFileService _mediaFileService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDiskProvider _diskProvider;
        private readonly Logger _logger;

        public BookImportService(IBookFileService bookFileService,
                                IBookService bookService,
                                IEditionService editionService,
                                IUpgradeMediaFiles bookFileUpgrader,
                                IMediaFileService mediaFileService,
                                IEventAggregator eventAggregator,
                                IDiskProvider diskProvider,
                                Logger logger)
        {
            _bookFileService = bookFileService;
            _bookService = bookService;
            _editionService = editionService;
            _bookFileUpgrader = bookFileUpgrader;
            _mediaFileService = mediaFileService;
            _eventAggregator = eventAggregator;
            _diskProvider = diskProvider;
            _logger = logger;
        }

        public List<ImportResult> Import(List<ImportDecision<LocalBook>> decisions, bool newDownload, DownloadClientItem downloadClientItem = null, ImportMode importMode = ImportMode.Auto)
        {
            var importResults = new List<ImportResult>();
            var allImportedBookFiles = new List<BookFile>();
            var allOldBookFiles = new List<BookFile>();

            foreach (var importDecision in decisions.Where(c => c.Approved))
            {
                var localBook = importDecision.Item;
                var oldFiles = new List<BookFile>();

                try
                {
                    // Check if we should upgrade existing file
                    var book = localBook.Book;
                    if (book == null)
                    {
                        importResults.Add(new ImportResult(importDecision, "Book not found"));
                        continue;
                    }

                    var edition = localBook.Edition ?? book.Editions.Value.FirstOrDefault(e => e.Monitored);
                    if (edition == null)
                    {
                        importResults.Add(new ImportResult(importDecision, "No monitored edition found"));
                        continue;
                    }

                    var bookFile = new BookFile
                    {
                        Path = localBook.Path.CleanFilePath(),
                        Size = localBook.Size,
                        Modified = localBook.Modified,
                        DateAdded = DateTime.UtcNow,
                        ReleaseGroup = localBook.ReleaseGroup,
                        Quality = localBook.Quality,
                        MediaInfo = localBook.MediaInfo,
                        BookId = book.Id,
                        AuthorId = book.AuthorId,
                        EditionId = edition.Id
                    };

                    bool copyOnly = importMode == ImportMode.PreferCopyOnly || 
                                   (importMode == ImportMode.Auto && localBook.ShouldImportExtra);

                    if (newDownload)
                    {
                        bookFile.SceneName = GetSceneName(downloadClientItem, localBook);

                        var moveResult = _bookFileUpgrader.UpgradeBookFile(bookFile, localBook, copyOnly);
                        oldFiles = moveResult.OldFiles;
                    }
                    else
                    {
                        bookFile.RelativePath = localBook.Author.Path.GetRelativePath(bookFile.Path);
                    }

                    _bookFileService.Add(bookFile);
                    importResults.Add(new ImportResult(importDecision));

                    if (newDownload)
                    {
                        _eventAggregator.PublishEvent(new BookImportedEvent(localBook, bookFile, oldFiles, newDownload, downloadClientItem));
                    }

                    allImportedBookFiles.Add(bookFile);
                    allOldBookFiles.AddRange(oldFiles);

                    _logger.Info("[{0}] Imported {1}", localBook.Author.Name, bookFile.RelativePath);
                }
                catch (RootFolderNotFoundException e)
                {
                    _logger.Warn(e, "Couldn't import book {0}", localBook);
                    importResults.Add(new ImportResult(importDecision, "Failed to import book, Root folder missing."));
                }
                catch (DestinationAlreadyExistsException e)
                {
                    _logger.Warn(e, "Couldn't import book {0}", localBook);
                    importResults.Add(new ImportResult(importDecision, "Failed to import book, Destination already exists."));
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger.Warn(e, "Couldn't import book {0}", localBook);
                    importResults.Add(new ImportResult(importDecision, "Failed to import book, Permissions error"));
                }
                catch (Exception e)
                {
                    _logger.Warn(e, "Couldn't import book {0}", localBook);
                    importResults.Add(new ImportResult(importDecision, "Failed to import book"));
                }
            }

            if (allImportedBookFiles.Any())
            {
                _eventAggregator.PublishEvent(new BookFileImportedEvent(allImportedBookFiles, allOldBookFiles, newDownload));
            }

            // Add all the rejected decisions
            importResults.AddRange(decisions.Where(c => !c.Approved)
                                           .Select(d => new ImportResult(d, d.Rejections.Select(r => r.Reason).ToArray())));

            return importResults;
        }

        private string GetSceneName(DownloadClientItem downloadClientItem, LocalBook localBook)
        {
            if (downloadClientItem != null)
            {
                var title = Parser.Parser.RemoveFileExtension(downloadClientItem.Title);

                if (downloadClientItem.DownloadClientInfo.Type != DownloadClientType.Usenet && 
                    title == Path.GetFileNameWithoutExtension(localBook.Path))
                {
                    return title;
                }
            }

            var fileName = Path.GetFileNameWithoutExtension(localBook.Path.CleanFilePath());

            if (localBook.SceneSource)
            {
                return fileName;
            }

            return null;
        }
    }

    public enum ImportMode
    {
        Auto,
        Copy,
        Move,
        PreferCopyOnly
    }
}