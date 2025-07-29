using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Common.Instrumentation.Extensions;
using Readarr.Core.Books.Commands;
using Readarr.Core.Books.Events;
using Readarr.Core.Exceptions;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.MetadataSource;
using Readarr.Core.Parser;

namespace Readarr.Core.Books
{
    public interface IRefreshBookService
    {
        bool RefreshBookInfo(Book book, Book remoteBook, bool forceUpdateFileTags);
        bool RefreshBookInfo(List<Book> books, Book remoteBook, bool forceUpdateFileTags);
    }

    public class RefreshBookService : IRefreshBookService, IExecute<RefreshBookCommand>
    {
        private readonly IBookService _bookService;
        private readonly IBookMetadataService _bookMetadataService;
        private readonly IEditionService _editionService;
        private readonly IProvideBookInfo _bookInfo;
        private readonly IMediaFileService _mediaFileService;
        private readonly IMapCoversToLocal _mediaCoverService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ICheckIfBookShouldBeRefreshed _checkIfBookShouldBeRefreshed;
        private readonly Logger _logger;

        public RefreshBookService(IBookService bookService,
                                 IBookMetadataService bookMetadataService,
                                 IEditionService editionService,
                                 IProvideBookInfo bookInfo,
                                 IMediaFileService mediaFileService,
                                 IMapCoversToLocal mediaCoverService,
                                 IEventAggregator eventAggregator,
                                 ICheckIfBookShouldBeRefreshed checkIfBookShouldBeRefreshed,
                                 Logger logger)
        {
            _bookService = bookService;
            _bookMetadataService = bookMetadataService;
            _editionService = editionService;
            _bookInfo = bookInfo;
            _mediaFileService = mediaFileService;
            _mediaCoverService = mediaCoverService;
            _eventAggregator = eventAggregator;
            _checkIfBookShouldBeRefreshed = checkIfBookShouldBeRefreshed;
            _logger = logger;
        }

        public bool RefreshBookInfo(Book book, Book remoteBook, bool forceUpdateFileTags)
        {
            var updated = false;

            _logger.ProgressInfo("Updating info for {0}", book.Metadata.Value?.Title ?? "Unknown");

            Book bookInfo;

            if (remoteBook == null)
            {
                try
                {
                    bookInfo = _bookInfo.GetBookInfo(book.Metadata.Value.ForeignBookId);
                }
                catch (BookNotFoundException)
                {
                    _logger.Error($"Book '{book.Metadata.Value?.Title}' (GoodreadsId {book.Metadata.Value.ForeignBookId}) was not found, it may have been removed from Goodreads.");
                    
                    _eventAggregator.PublishEvent(new BookDeletedEvent(book, false, false));
                    
                    return false;
                }
            }
            else
            {
                bookInfo = remoteBook;
            }

            if (book.Metadata.Value.ForeignBookId != bookInfo.Metadata.Value.ForeignBookId)
            {
                _logger.Warn($"Book '{book.Metadata.Value?.Title}' (GoodreadsId {book.Metadata.Value.ForeignBookId}) was replaced with '{bookInfo.Metadata.Value?.Title}' (GoodreadsId {bookInfo.Metadata.Value.ForeignBookId})");
                
                book.Metadata.Value.ForeignBookId = bookInfo.Metadata.Value.ForeignBookId;
            }

            // Update book metadata
            book.Metadata.Value.ApplyChanges(bookInfo.Metadata.Value);
            book.Metadata = _bookMetadataService.Upsert(book.Metadata.Value);

            // Update book properties
            book.LastInfoSync = DateTime.UtcNow;
            book.CleanTitle = book.Metadata.Value.Title.CleanBookTitle();
            book.SortTitle = book.Metadata.Value.SortTitle;

            _mediaCoverService.ConvertToLocalUrls(book.Id, book.Metadata.Value.Images);

            // Update editions
            updated |= RefreshEditions(book, bookInfo.Editions);

            _bookService.UpdateBook(book);

            if (forceUpdateFileTags)
            {
                _logger.Debug("Updating file tags for book {0}", book.Metadata.Value?.Title ?? "Unknown");
                // TODO: Implement file tag updating
            }

            _logger.Debug("Finished book refresh for {0}", book.Metadata.Value?.Title ?? "Unknown");

            return updated;
        }

        public bool RefreshBookInfo(List<Book> books, Book remoteBook, bool forceUpdateFileTags)
        {
            var updated = false;

            foreach (var book in books)
            {
                updated |= RefreshBookInfo(book, remoteBook, forceUpdateFileTags);
            }

            return updated;
        }

        private bool RefreshEditions(Book book, List<Edition> remoteEditions)
        {
            var updated = false;
            var existingEditions = _editionService.GetEditions(book.Id);

            // Match remote editions to existing editions
            var editionMatches = new Dictionary<Edition, Edition>();
            
            foreach (var remoteEdition in remoteEditions)
            {
                var existingEdition = existingEditions.FirstOrDefault(e => e.ForeignEditionId == remoteEdition.ForeignEditionId);
                
                if (existingEdition != null)
                {
                    editionMatches[existingEdition] = remoteEdition;
                }
            }

            // Update existing editions
            foreach (var match in editionMatches)
            {
                var existingEdition = match.Key;
                var remoteEdition = match.Value;

                existingEdition.Title = remoteEdition.Title;
                existingEdition.Isbn = remoteEdition.Isbn;
                existingEdition.Isbn13 = remoteEdition.Isbn13;
                existingEdition.Asin = remoteEdition.Asin;
                existingEdition.Language = remoteEdition.Language;
                existingEdition.Overview = remoteEdition.Overview;
                existingEdition.Format = remoteEdition.Format;
                existingEdition.IsEbook = remoteEdition.IsEbook;
                existingEdition.Publisher = remoteEdition.Publisher;
                existingEdition.PageCount = remoteEdition.PageCount;
                existingEdition.ReleaseDate = remoteEdition.ReleaseDate;
                existingEdition.Images = remoteEdition.Images;
                existingEdition.Ratings = remoteEdition.Ratings;

                _editionService.UpdateEdition(existingEdition);
                updated = true;
            }

            // Add new editions
            var newEditions = remoteEditions.Where(e => !editionMatches.Values.Contains(e)).ToList();
            
            if (newEditions.Any())
            {
                _logger.Info($"Adding {newEditions.Count} new editions for book {book.Metadata.Value?.Title ?? "Unknown"}");
                
                foreach (var newEdition in newEditions)
                {
                    newEdition.BookId = book.Id;
                    newEdition.Monitored = book.Monitored;
                }

                _editionService.InsertMany(newEditions);
                updated = true;
            }

            // Remove editions that no longer exist
            var editionsToDelete = existingEditions.Where(e => !editionMatches.ContainsKey(e)).ToList();
            
            if (editionsToDelete.Any())
            {
                _logger.Info($"Removing {editionsToDelete.Count} editions for book {book.Metadata.Value?.Title ?? "Unknown"}");
                _editionService.DeleteMany(editionsToDelete);
                updated = true;
            }

            return updated;
        }

        public void Execute(RefreshBookCommand message)
        {
            var shouldRefresh = _checkIfBookShouldBeRefreshed.ShouldRefresh(message.BookId);

            if (message.BookId.HasValue)
            {
                var book = _bookService.GetBook(message.BookId.Value);
                
                if (shouldRefresh)
                {
                    try
                    {
                        RefreshBookInfo(book, null, message.ForceUpdateFileTags);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, $"Couldn't refresh info for {book.Metadata.Value?.Title ?? "Unknown"}");
                        throw;
                    }
                }
                else
                {
                    _logger.Info($"Skipping refresh of book: {book.Metadata.Value?.Title ?? "Unknown"}");
                }
            }
            else
            {
                var books = _bookService.GetAllBooks().OrderBy(b => b.Metadata.Value?.Title).ToList();

                foreach (var book in books)
                {
                    if (shouldRefresh)
                    {
                        try
                        {
                            RefreshBookInfo(book, null, message.ForceUpdateFileTags);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, $"Couldn't refresh info for {book.Metadata.Value?.Title ?? "Unknown"}");
                        }
                    }
                    else
                    {
                        _logger.Info($"Skipping refresh of book: {book.Metadata.Value?.Title ?? "Unknown"}");
                    }
                }
            }
        }
    }
}