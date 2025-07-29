using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Api.V1.Book;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Http;
using Readarr.Http.Extensions;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;
using Readarr.SignalR;

namespace Readarr.Api.V1.BookSeries
{
    [V3ApiController("series/book")]
    public class BookSeriesController : RestControllerWithSignalR<BookSeriesResource, Core.Books.Series>
    {
        private readonly ISeriesService _seriesService;
        private readonly IBookService _bookService;
        private readonly IManageCommandQueue _commandQueueManager;

        public BookSeriesController(IBroadcastSignalRMessage signalRBroadcaster,
                                  ISeriesService seriesService,
                                  IBookService bookService,
                                  IManageCommandQueue commandQueueManager)
            : base(signalRBroadcaster)
        {
            _seriesService = seriesService;
            _bookService = bookService;
            _commandQueueManager = commandQueueManager;

            PostValidator.RuleFor(s => s.ForeignSeriesId).NotEmpty();
            PostValidator.RuleFor(s => s.Title).NotEmpty();
            PostValidator.RuleFor(s => s.AuthorId).GreaterThan(0);
        }

        protected override BookSeriesResource GetResourceById(int id)
        {
            var series = _seriesService.GetSeries(id);
            return GetSeriesResource(series);
        }

        [HttpGet]
        public List<BookSeriesResource> GetSeries([FromQuery]int? authorId)
        {
            List<BookSeriesResource> seriesResources;

            if (authorId.HasValue)
            {
                seriesResources = _seriesService.GetByAuthorId(authorId.Value).ToResource();
            }
            else
            {
                seriesResources = _seriesService.GetAllSeries().ToResource();
            }

            // Populate book links
            foreach (var resource in seriesResources)
            {
                PopulateBooks(resource);
            }

            return seriesResources;
        }

        [RestPostById]
        public ActionResult<BookSeriesResource> AddSeries([FromBody] BookSeriesResource seriesResource)
        {
            var series = _seriesService.AddSeries(seriesResource.ToModel());

            return Created(series.Id);
        }

        [RestPutById]
        public ActionResult<BookSeriesResource> UpdateSeries([FromBody] BookSeriesResource seriesResource)
        {
            var series = _seriesService.GetSeries(seriesResource.Id);

            var model = seriesResource.ToModel();
            model.Id = series.Id;

            _seriesService.UpdateSeries(model);

            BroadcastResourceChange(ModelAction.Updated, GetSeriesResource(model));

            return Accepted(seriesResource);
        }

        [RestDeleteById]
        public void DeleteSeries(int id)
        {
            _seriesService.DeleteSeries(id);
        }

        [HttpPost("{id}/link")]
        public IActionResult LinkBookToSeries(int id, [FromBody] SeriesBookLinkResource linkResource)
        {
            _seriesService.LinkBookToSeries(id, linkResource.BookId, linkResource.Position);
            return Accepted();
        }

        [HttpDelete("{id}/link/{bookId}")]
        public IActionResult UnlinkBookFromSeries(int id, int bookId)
        {
            _seriesService.UnlinkBookFromSeries(id, bookId);
            return Accepted();
        }

        private BookSeriesResource GetSeriesResource(Core.Books.Series series)
        {
            if (series == null)
            {
                return null;
            }

            var resource = series.ToResource();
            PopulateBooks(resource);

            return resource;
        }

        private void PopulateBooks(BookSeriesResource resource)
        {
            var links = _seriesService.GetSeriesBookLinks(resource.Id);
            resource.Books = links.ToResource();

            // Optionally populate book details
            foreach (var bookLink in resource.Books)
            {
                bookLink.Book = _bookService.GetBook(bookLink.BookId).ToResource(false);
            }
        }
    }

    public class SeriesBookLinkResource
    {
        public int BookId { get; set; }
        public string Position { get; set; }
    }
}