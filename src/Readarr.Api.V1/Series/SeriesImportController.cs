using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Books;
using Readarr.Http;

namespace Readarr.Api.V3.Series
{
    [V3ApiController("series/import")]
    public class AuthorImportController : Controller
    {
        private readonly IAddAuthorService _addAuthorService;

        public SeriesImportController(IAddAuthorService addAuthorService)
        {
            _addAuthorService = addAuthorService;
        }

        [HttpPost]
        public object Import([FromBody] List<SeriesResource> resource)
        {
            var newSeries = resource.ToModel();

            return _addAuthorService.AddSeries(newSeries).ToResource();
        }
    }
}
