using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Events;
using Readarr.Http;
using Readarr.Http.Extensions;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;
using Readarr.SignalR;

namespace Readarr.Api.V1.Edition
{
    [V3ApiController]
    public class EditionController : RestControllerWithSignalR<EditionResource, Core.Books.Edition>,
                                    IHandle<BookFileDeletedEvent>
    {
        private readonly IEditionService _editionService;
        private readonly IBookService _bookService;
        private readonly IUpgradableSpecification _upgradableSpecification;

        public EditionController(IEditionService editionService,
                               IBookService bookService,
                               IUpgradableSpecification upgradableSpecification,
                               IBroadcastSignalRMessage signalRBroadcaster)
            : base(signalRBroadcaster)
        {
            _editionService = editionService;
            _bookService = bookService;
            _upgradableSpecification = upgradableSpecification;
        }

        protected override EditionResource GetResourceById(int id)
        {
            return _editionService.GetEdition(id).ToResource();
        }

        [HttpGet]
        public List<EditionResource> GetEditions([FromQuery]int? bookId)
        {
            if (bookId.HasValue)
            {
                return _editionService.GetEditions(bookId.Value).ToResource();
            }

            return _editionService.GetAllEditions().ToResource();
        }

        [RestPutById]
        public ActionResult<EditionResource> UpdateEdition([FromBody] EditionResource editionResource)
        {
            var edition = _editionService.GetEdition(editionResource.Id);
            var model = editionResource.ToModel();
            model.Id = edition.Id;

            _editionService.UpdateEdition(model);

            BroadcastResourceChange(ModelAction.Updated, model.ToResource());

            return Accepted(model.ToResource());
        }

        [HttpPut("monitor")]
        public IActionResult SetEditionsMonitored([FromBody] EditionsMonitoredResource resource)
        {
            _editionService.SetMonitored(resource.EditionIds, resource.Monitored);

            return Accepted();
        }

        [NonAction]
        public void Handle(BookFileDeletedEvent message)
        {
            // Update any editions that had this file
            var edition = _editionService.GetEditionByFileId(message.BookFile.Id);
            if (edition != null)
            {
                BroadcastResourceChange(ModelAction.Updated, edition.ToResource());
            }
        }
    }

    public class EditionsMonitoredResource
    {
        public List<int> EditionIds { get; set; }
        public bool Monitored { get; set; }
    }
}