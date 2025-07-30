using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Api.V1.Book;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Http;
using Readarr.Http.Extensions;

namespace Readarr.Api.V1.Wanted
{
    [V1ApiController("wanted/missing")]
    public class MissingBookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IEventAggregator _eventAggregator;

        public MissingBookController(IBookService bookService, IEventAggregator eventAggregator)
        {
            _bookService = bookService;
            _eventAggregator = eventAggregator;
        }

        [HttpGet]
        public PagingResource<BookResource> GetMissingBooks([FromQuery] PagingRequestResource pagingResource)
        {
            var page = pagingResource.Page ?? 1;
            var pageSize = pagingResource.PageSize ?? 50;
            var sortKey = pagingResource.SortKey ?? "releaseDate";
            var sortDirection = pagingResource.SortDirection ?? "desc";

            var pagingSpec = new PagingSpec<Book>
            {
                Page = page,
                PageSize = pageSize,
                SortKey = sortKey,
                SortDirection = sortDirection == "asc" ? SortDirection.Ascending : SortDirection.Descending
            };

            // Get books without files
            var missingBooks = _bookService.BooksWithoutFiles(pagingSpec);

            var response = new PagingResource<BookResource>
            {
                Page = pagingSpec.Page,
                PageSize = pagingSpec.PageSize,
                SortKey = pagingSpec.SortKey,
                SortDirection = pagingSpec.SortDirection.ToString().ToLowerInvariant(),
                TotalRecords = pagingSpec.TotalRecords,
                Records = missingBooks.Records.ToResource()
            };

            return response;
        }

        [HttpGet("{id}")]
        public BookResource GetMissingBook(int id)
        {
            var book = _bookService.GetBook(id);
            
            if (book == null || book.BookFileId > 0)
            {
                return null;
            }

            return book.ToResource();
        }
    }
}