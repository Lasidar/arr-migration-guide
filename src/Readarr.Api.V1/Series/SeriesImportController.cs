using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Books;
using Readarr.Http;

namespace Readarr.Api.V3.Series
{
    [V3ApiController("series/import")]
    public class SeriesImportController : Controller
    {
        private readonly IAddSeriesService _addSeriesService;

        public SeriesImportController(IAddSeriesService addSeriesService)
        {
            _addSeriesService = addSeriesService;
        }

        [HttpPost]
        public object Import([FromBody] List<SeriesResource> resource)
        {
            var newSeries = resource.ToModel();

            return _addSeriesService.AddSeries(newSeries).ToResource();
        }
    }
}
