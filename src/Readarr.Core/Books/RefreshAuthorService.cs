using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Common.Extensions;
using Readarr.Core.Books.Commands;
using Readarr.Core.Books.Events;
using Readarr.Core.Exceptions;
using Readarr.Core.History;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.MetadataSource;

namespace Readarr.Core.Books
{
    public interface IRefreshAuthorService
    {
        bool RefreshAuthorInfo(Author author, List<Book> remoteBooks, Author remoteAuthor, bool forceUpdateFileTags);
        bool RefreshAuthorInfo(List<Author> authors, List<Book> remoteBooks, Author remoteAuthor, bool forceUpdateFileTags);
    }

    public class RefreshAuthorService : IRefreshAuthorService, IExecute<RefreshAuthorCommand>
    {
        private readonly IProvideAuthorInfo _authorInfo;
        private readonly IAuthorService _authorService;
        private readonly IAuthorMetadataService _authorMetadataService;
        private readonly IBookService _bookService;
        private readonly IRefreshBookService _refreshBookService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IManageCommandQueue _commandQueueManager;
        private readonly IMediaFileService _mediaFileService;
        private readonly IHistoryService _historyService;
        private readonly IRootFolderWatchingService _rootFolderWatchingService;
        private readonly ICheckIfAuthorShouldBeRefreshed _checkIfAuthorShouldBeRefreshed;
        private readonly IMapCoversToLocal _mediaCoverService;
        private readonly Logger _logger;

        public RefreshAuthorService(IProvideAuthorInfo authorInfo,
                                   IAuthorService authorService,
                                   IAuthorMetadataService authorMetadataService,
                                   IBookService bookService,
                                   IRefreshBookService refreshBookService,
                                   IEventAggregator eventAggregator,
                                   IManageCommandQueue commandQueueManager,
                                   IMediaFileService mediaFileService,
                                   IHistoryService historyService,
                                   IRootFolderWatchingService rootFolderWatchingService,
                                   ICheckIfAuthorShouldBeRefreshed checkIfAuthorShouldBeRefreshed,
                                   IMapCoversToLocal mediaCoverService,
                                   Logger logger)
        {
            _authorInfo = authorInfo;
            _authorService = authorService;
            _authorMetadataService = authorMetadataService;
            _bookService = bookService;
            _refreshBookService = refreshBookService;
            _eventAggregator = eventAggregator;
            _commandQueueManager = commandQueueManager;
            _mediaFileService = mediaFileService;
            _historyService = historyService;
            _rootFolderWatchingService = rootFolderWatchingService;
            _checkIfAuthorShouldBeRefreshed = checkIfAuthorShouldBeRefreshed;
            _mediaCoverService = mediaCoverService;
            _logger = logger;
        }

        public bool RefreshAuthorInfo(Author author, List<Book> remoteBooks, Author remoteAuthor, bool forceUpdateFileTags)
        {
            var updated = false;

            _logger.ProgressInfo("Updating info for {0}", author.Name);

            Author authorInfo;
            List<Book> books;

            if (remoteAuthor == null)
            {
                try
                {
                    var tuple = _authorInfo.GetAuthorInfo(author.Metadata.Value.ForeignAuthorId);
                    authorInfo = tuple.Item1;
                    books = tuple.Item2;
                }
                catch (AuthorNotFoundException)
                {
                    _logger.Error($"Author '{author.Name}' (GoodreadsId {author.Metadata.Value.ForeignAuthorId}) was not found, it may have been removed from Goodreads.");
                    
                    author.Metadata.Value.Status = AuthorStatusType.Ended;
                    _authorMetadataService.Upsert(author.Metadata.Value);
                    
                    _eventAggregator.PublishEvent(new AuthorDeletedEvent(author, false, false));
                    
                    return false;
                }
            }
            else
            {
                authorInfo = remoteAuthor;
                books = remoteBooks;
            }

            if (author.Metadata.Value.ForeignAuthorId != authorInfo.Metadata.Value.ForeignAuthorId)
            {
                _logger.Warn($"Author '{author.Name}' (GoodreadsId {author.Metadata.Value.ForeignAuthorId}) was replaced with '{authorInfo.Name}' (GoodreadsId {authorInfo.Metadata.Value.ForeignAuthorId})");
                
                author.Metadata.Value.ForeignAuthorId = authorInfo.Metadata.Value.ForeignAuthorId;
            }

            // Update author metadata
            author.Metadata.Value.ApplyChanges(authorInfo.Metadata.Value);
            author.Metadata = _authorMetadataService.Upsert(author.Metadata.Value);

            // Update author properties
            author.LastInfoSync = DateTime.UtcNow;
            author.Path = authorInfo.Path ?? author.Path;
            author.QualityProfileId = authorInfo.QualityProfileId;
            author.MetadataProfileId = authorInfo.MetadataProfileId;
            author.Tags = authorInfo.Tags;
            author.Monitored = authorInfo.Monitored;

            _mediaCoverService.ConvertToLocalUrls(author.Id, MediaCoverEntity.Author, author.Metadata.Value.Images);

            // Update books
            updated |= RefreshBooks(author, books, forceUpdateFileTags);

            _authorService.UpdateAuthor(author);

            _logger.Debug("Finished author refresh for {0}", author.Name);

            return updated;
        }

        public bool RefreshAuthorInfo(List<Author> authors, List<Book> remoteBooks, Author remoteAuthor, bool forceUpdateFileTags)
        {
            var updated = false;

            foreach (var author in authors)
            {
                updated |= RefreshAuthorInfo(author, remoteBooks, remoteAuthor, forceUpdateFileTags);
            }

            return updated;
        }

        private bool RefreshBooks(Author author, List<Book> remoteBooks, bool forceUpdateFileTags)
        {
            var updated = false;
            var existingBooks = _bookService.GetBooksByAuthor(author.Id);

            // Match remote books to existing books
            var bookMatches = new Dictionary<Book, Book>();
            
            foreach (var remoteBook in remoteBooks)
            {
                var existingBook = existingBooks.FirstOrDefault(b => b.Metadata.Value.ForeignBookId == remoteBook.Metadata.Value.ForeignBookId);
                
                if (existingBook != null)
                {
                    bookMatches[existingBook] = remoteBook;
                }
            }

            // Update existing books
            foreach (var match in bookMatches)
            {
                var existingBook = match.Key;
                var remoteBook = match.Value;

                updated |= _refreshBookService.RefreshBookInfo(existingBook, remoteBook, forceUpdateFileTags);
            }

            // Add new books
            var newBooks = remoteBooks.Where(b => !bookMatches.Values.Contains(b)).ToList();
            
            if (newBooks.Any())
            {
                _logger.Info($"Adding {newBooks.Count} new books for author {author.Name}");
                
                foreach (var newBook in newBooks)
                {
                    newBook.AuthorId = author.Id;
                    newBook.AuthorMetadataId = author.AuthorMetadataId;
                    newBook.Added = DateTime.UtcNow;
                    newBook.LastInfoSync = DateTime.UtcNow;
                    newBook.Monitored = author.Monitored;
                }

                _bookService.AddBooks(newBooks);
                updated = true;
            }

            // Remove books that no longer exist
            var booksToDelete = existingBooks.Where(b => !bookMatches.ContainsKey(b)).ToList();
            
            if (booksToDelete.Any())
            {
                _logger.Info($"Removing {booksToDelete.Count} books for author {author.Name}");
                _bookService.DeleteBooks(booksToDelete.Select(b => b.Id).ToList(), false, false);
                updated = true;
            }

            return updated;
        }

        public void Execute(RefreshAuthorCommand message)
        {
            var shouldRefresh = _checkIfAuthorShouldBeRefreshed.ShouldRefresh(message.AuthorId);

            if (message.AuthorId.HasValue)
            {
                var author = _authorService.GetAuthor(message.AuthorId.Value);
                
                if (shouldRefresh)
                {
                    try
                    {
                        RefreshAuthorInfo(author, null, null, message.ForceUpdateFileTags);
                    }
                    catch (Exception e)
                    {
                        _logger.Error(e, $"Couldn't refresh info for {author.Name}");
                        throw;
                    }
                }
                else
                {
                    _logger.Info($"Skipping refresh of author: {author.Name}");
                }
            }
            else
            {
                var authors = _authorService.GetAllAuthors().OrderBy(a => a.Name).ToList();

                foreach (var author in authors)
                {
                    if (shouldRefresh)
                    {
                        try
                        {
                            RefreshAuthorInfo(author, null, null, message.ForceUpdateFileTags);
                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, $"Couldn't refresh info for {author.Name}");
                        }
                    }
                    else
                    {
                        _logger.Info($"Skipping refresh of author: {author.Name}");
                    }
                }
            }
        }
    }

    public interface ICheckIfAuthorShouldBeRefreshed
    {
        bool ShouldRefresh(int? authorId);
    }

    public interface IRootFolderWatchingService
    {
    }

    public interface IMediaFileService
    {
    }

    public interface IHistoryService
    {
    }
}