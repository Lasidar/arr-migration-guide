using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.Books.Commands;
using Readarr.Core.Datastore;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.MediaCover;
using Readarr.Core.Messaging.Commands;
using Readarr.Http;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;

namespace Readarr.Api.V1.Books
{
    [V1ApiController]
    public class BookController : RestController<BookResource>
    {
        private readonly IBookService _bookService;
        private readonly IAddBookService _addBookService;
        private readonly IMapCoversToLocal _coverMapper;
        private readonly IManageCommandQueue _commandQueueManager;
        private readonly IUpgradableSpecification _upgradableSpecification;

        public BookController(IBookService bookService,
                             IAddBookService addBookService,
                             IMapCoversToLocal coverMapper,
                             IManageCommandQueue commandQueueManager,
                             IUpgradableSpecification upgradableSpecification)
        {
            _bookService = bookService;
            _addBookService = addBookService;
            _coverMapper = coverMapper;
            _commandQueueManager = commandQueueManager;
            _upgradableSpecification = upgradableSpecification;
        }

        protected override BookResource GetResourceById(int id)
        {
            var book = _bookService.GetBook(id);
            return MapToResource(book);
        }

        [HttpGet]
        public List<BookResource> GetBooks([FromQuery] int? authorId, [FromQuery] bool? includeAllAuthorBooks = false)
        {
            var books = authorId.HasValue 
                ? _bookService.GetBooksByAuthor(authorId.Value) 
                : _bookService.GetAllBooks();

            var resources = MapToResource(books);

            return resources;
        }

        [RestPostById]
        public ActionResult<BookResource> AddBook(BookResource bookResource)
        {
            var book = bookResource.ToModel();
            book = _addBookService.AddBook(book);

            return Created(MapToResource(book));
        }

        [RestPutById]
        public ActionResult<BookResource> UpdateBook(BookResource bookResource)
        {
            var book = _bookService.GetBook(bookResource.Id);
            
            var model = bookResource.ToModel(book);
            
            _bookService.UpdateBook(model);

            BroadcastResourceChange(ModelAction.Updated, MapToResource(model));

            return Accepted(MapToResource(model));
        }

        [RestDeleteById]
        public void DeleteBook(int id, [FromQuery] bool deleteFiles = false, [FromQuery] bool addImportListExclusion = false)
        {
            _bookService.DeleteBook(id, deleteFiles, addImportListExclusion);
        }

        [HttpPost("monitor")]
        public IActionResult SetBooksMonitored([FromBody] BooksMonitoredResource resource)
        {
            _bookService.SetBookMonitored(resource.BookIds, resource.Monitored);

            return Accepted(MapToResource(_bookService.GetBooks(resource.BookIds)));
        }

        private BookResource MapToResource(Book book)
        {
            if (book == null) return null;

            var resource = book.ToResource();
            
            FetchAndLinkBookStatistics(resource);
            LinkCoverImages(resource);
            
            return resource;
        }

        private List<BookResource> MapToResource(IEnumerable<Book> books)
        {
            var resources = books.Select(MapToResource).ToList();
            return resources;
        }

        private void FetchAndLinkBookStatistics(BookResource resource)
        {
            // TODO: Link book statistics like file count, size on disk
            resource.Statistics = new BookStatisticsResource
            {
                BookFileCount = 0,
                BookCount = 1,
                TotalBookCount = 1,
                SizeOnDisk = 0,
                PercentOfBooks = 100
            };
        }

        private void LinkCoverImages(BookResource resource)
        {
            if (resource.Images != null)
            {
                _coverMapper.ConvertToLocalUrls(resource.Id, MediaCoverEntity.Book, resource.Images);
            }
        }
    }
}