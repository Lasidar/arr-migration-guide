using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.Datastore.Events;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Http;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;
using Readarr.SignalR;

namespace Readarr.Api.V1.BookFiles
{
    [V1ApiController]
    public class BookFileController : RestControllerWithSignalR<BookFileResource, BookFile>,
                                     IHandle<BookFileAddedEvent>,
                                     IHandle<BookFileDeletedEvent>
    {
        private readonly IMediaFileService _mediaFileService;
        private readonly IDeleteMediaFiles _deleteMediaFiles;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IUpgradableSpecification _upgradableSpecification;

        public BookFileController(IBroadcastSignalRMessage signalRBroadcaster,
                                 IMediaFileService mediaFileService,
                                 IDeleteMediaFiles deleteMediaFiles,
                                 IBookService bookService,
                                 IAuthorService authorService,
                                 IUpgradableSpecification upgradableSpecification)
            : base(signalRBroadcaster)
        {
            _mediaFileService = mediaFileService;
            _deleteMediaFiles = deleteMediaFiles;
            _bookService = bookService;
            _authorService = authorService;
            _upgradableSpecification = upgradableSpecification;
        }

        protected override BookFileResource GetResourceById(int id)
        {
            var bookFile = _mediaFileService.Get(id);
            var book = _bookService.GetBook(bookFile.BookId);
            var author = _authorService.GetAuthor(book.AuthorId);

            return bookFile.ToResource(author, book, _upgradableSpecification);
        }

        [HttpGet]
        public List<BookFileResource> GetBookFiles([FromQuery] int? authorId, [FromQuery] List<int> bookFileIds, [FromQuery] List<int> bookIds, [FromQuery] bool? unmapped)
        {
            if (!authorId.HasValue && !bookFileIds.Any() && !bookIds.Any() && !unmapped.HasValue)
            {
                throw new BadRequestException("authorId, bookFileIds, bookIds or unmapped must be provided");
            }

            if (unmapped.HasValue && unmapped.Value)
            {
                var files = _mediaFileService.GetUnmappedFiles();
                return files.ConvertAll(f => MapToResource(f));
            }

            if (authorId.HasValue && !bookFileIds.Any() && !bookIds.Any())
            {
                var author = _authorService.GetAuthor(authorId.Value);
                var books = _bookService.GetBooksByAuthor(authorId.Value);
                
                return books.SelectMany(b => _mediaFileService.GetFilesByBook(b.Id))
                           .Select(f => f.ToResource(author, books.First(b => b.Id == f.BookId), _upgradableSpecification))
                           .ToList();
            }

            if (bookIds.Any())
            {
                var result = new List<BookFileResource>();
                foreach (var bookId in bookIds)
                {
                    var book = _bookService.GetBook(bookId);
                    var author = _authorService.GetAuthor(book.AuthorId);
                    var files = _mediaFileService.GetFilesByBook(bookId);
                    result.AddRange(files.Select(f => f.ToResource(author, book, _upgradableSpecification)));
                }
                return result;
            }

            // Get by specific file IDs
            var bookFiles = bookFileIds.Select(id => _mediaFileService.Get(id)).ToList();
            return bookFiles.Select(f =>
            {
                var book = _bookService.GetBook(f.BookId);
                var author = _authorService.GetAuthor(book.AuthorId);
                return f.ToResource(author, book, _upgradableSpecification);
            }).ToList();
        }

        [RestPutById]
        public ActionResult<BookFileResource> UpdateBookFile(BookFileResource bookFileResource)
        {
            var bookFile = _mediaFileService.Get(bookFileResource.Id);
            bookFile.Quality = bookFileResource.Quality;
            bookFile.Languages = bookFileResource.Languages;
            bookFile.ReleaseGroup = bookFileResource.ReleaseGroup;
            bookFile.SceneName = bookFileResource.SceneName;
            
            _mediaFileService.Update(bookFile);
            
            return Accepted(bookFileResource.Id);
        }

        [HttpDelete("{id}")]
        public void DeleteBookFile(int id)
        {
            var bookFile = _mediaFileService.Get(id);
            var book = _bookService.GetBook(bookFile.BookId);
            var author = _authorService.GetAuthor(book.AuthorId);
            var fullPath = System.IO.Path.Combine(author.Path, bookFile.RelativePath);

            _deleteMediaFiles.DeleteBookFile(book, bookFile);
        }

        [HttpDelete("bulk")]
        public IActionResult DeleteBookFiles([FromBody] BookFileListResource resource)
        {
            var bookFiles = _mediaFileService.Get(resource.BookFileIds);

            foreach (var bookFile in bookFiles)
            {
                var book = _bookService.GetBook(bookFile.BookId);
                _deleteMediaFiles.DeleteBookFile(book, bookFile);
            }

            return Ok();
        }

        private BookFileResource MapToResource(BookFile bookFile)
        {
            var book = bookFile.BookId > 0 ? _bookService.GetBook(bookFile.BookId) : null;
            var author = book?.AuthorId > 0 ? _authorService.GetAuthor(book.AuthorId) : null;

            return bookFile.ToResource(author, book, _upgradableSpecification);
        }

        public void Handle(BookFileAddedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, MapToResource(message.BookFile));
        }

        public void Handle(BookFileDeletedEvent message)
        {
            BroadcastResourceChange(ModelAction.Deleted, MapToResource(message.BookFile));
        }
    }
}