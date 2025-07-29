using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Books.Commands;
using Readarr.Core.Books.Events;
using Readarr.Core.Datastore;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Validation;
using Readarr.Http;
using Readarr.Http.Extensions;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;
using Readarr.SignalR;

namespace Readarr.Api.V1.Book
{
    [V3ApiController]
    public class BookController : RestControllerWithSignalR<BookResource, Core.Books.Book>,
                                IHandle<BookAddedEvent>,
                                IHandle<BookEditedEvent>,
                                IHandle<BookDeletedEvent>,
                                IHandle<BookFileDeletedEvent>,
                                IHandle<MediaCoversUpdatedEvent>
    {
        private readonly IBookService _bookService;
        private readonly IAddBookService _addBookService;
        private readonly IBookStatisticsService _bookStatisticsService;
        private readonly IMapCoversToLocal _coverMapper;
        private readonly IManageCommandQueue _commandQueueManager;

        public BookController(IBroadcastSignalRMessage signalRBroadcaster,
                            IBookService bookService,
                            IAddBookService addBookService,
                            IBookStatisticsService bookStatisticsService,
                            IMapCoversToLocal coverMapper,
                            IManageCommandQueue commandQueueManager,
                            BookExistsValidator bookExistsValidator,
                            BookAncestorValidator bookAncestorValidator)
            : base(signalRBroadcaster)
        {
            _bookService = bookService;
            _addBookService = addBookService;
            _bookStatisticsService = bookStatisticsService;
            _coverMapper = coverMapper;
            _commandQueueManager = commandQueueManager;

            Http.Validation.RuleBuilderExtensions.ValidId(SharedValidator.RuleFor(s => s.AuthorId));

            PostValidator.RuleFor(s => s.ForeignBookId).NotEmpty();
            PostValidator.RuleFor(s => s.AuthorId).NotEmpty().SetValidator(bookExistsValidator);
        }

        protected override BookResource GetResourceById(int id)
        {
            var book = _bookService.GetBook(id);
            return GetBookResource(book);
        }

        [HttpGet]
        public List<BookResource> GetBooks([FromQuery]int? authorId)
        {
            var bookStats = _bookStatisticsService.BookStatistics();
            List<BookResource> bookResources;

            if (authorId.HasValue)
            {
                bookResources = _bookService.GetBooksByAuthor(authorId.Value).ToResource();
            }
            else
            {
                bookResources = _bookService.GetAllBooks().ToResource();
            }

            MapCoversToLocal(bookResources.ToArray());
            LinkBookStatistics(bookResources, bookStats);

            return bookResources;
        }

        [RestPostById]
        public ActionResult<BookResource> AddBook([FromBody] BookResource bookResource)
        {
            var book = _addBookService.AddBook(bookResource.ToModel());

            return Created(book.Id);
        }

        [RestPutById]
        public ActionResult<BookResource> UpdateBook([FromBody] BookResource bookResource)
        {
            var book = _bookService.GetBook(bookResource.Id);

            var model = bookResource.ToModel();
            model.Id = book.Id;

            _bookService.UpdateBook(model);

            BroadcastResourceChange(ModelAction.Updated, bookResource);

            return Accepted(bookResource);
        }

        [RestDeleteById]
        public void DeleteBook(int id, [FromQuery] bool deleteFiles = false, [FromQuery] bool addImportListExclusion = false)
        {
            _bookService.DeleteBook(id, deleteFiles, addImportListExclusion);
        }

        [HttpPut("monitor")]
        public IActionResult SetBooksMonitored([FromBody] BooksMonitoredResource resource)
        {
            _bookService.SetBooksMonitored(resource.BookIds, resource.Monitored);

            return Accepted();
        }

        private BookResource GetBookResource(Core.Books.Book book)
        {
            if (book == null)
            {
                return null;
            }

            var resource = book.ToResource();
            MapCoversToLocal(resource);

            return resource;
        }

        private void MapCoversToLocal(params BookResource[] books)
        {
            foreach (var bookResource in books)
            {
                _coverMapper.ConvertToLocalUrls(bookResource.Id, MediaCoverEntity.Book, bookResource.Images);
            }
        }

        private void LinkBookStatistics(List<BookResource> resources, List<BookStatistics> bookStatistics)
        {
            foreach (var book in resources)
            {
                var stats = bookStatistics.SingleOrDefault(ss => ss.BookId == book.Id);
                if (stats != null)
                {
                    book.Statistics = new BookStatisticsResource
                    {
                        BookFileCount = stats.BookFileCount,
                        EditionCount = stats.EditionCount,
                        TotalEditionCount = stats.TotalEditionCount,
                        SizeOnDisk = stats.SizeOnDisk
                    };
                }
            }
        }

        [NonAction]
        public void Handle(BookAddedEvent message)
        {
            BroadcastResourceChange(ModelAction.Created, message.Book.ToResource());
        }

        [NonAction]
        public void Handle(BookEditedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, message.Book.ToResource());
        }

        [NonAction]
        public void Handle(BookDeletedEvent message)
        {
            BroadcastResourceChange(ModelAction.Deleted, message.Book.ToResource());
        }

        [NonAction]
        public void Handle(BookFileDeletedEvent message)
        {
            if (message.BookFile.Book != null && message.BookFile.Book.IsLoaded)
            {
                BroadcastResourceChange(ModelAction.Updated, message.BookFile.Book.Value.ToResource());
            }
        }

        [NonAction]
        public void Handle(MediaCoversUpdatedEvent message)
        {
            if (message.Updated)
            {
                BroadcastResourceChange(ModelAction.Updated, _bookService.GetBook(message.Book.Id).ToResource());
            }
        }
    }

    public class BooksMonitoredResource
    {
        public List<int> BookIds { get; set; }
        public bool Monitored { get; set; }
    }
}