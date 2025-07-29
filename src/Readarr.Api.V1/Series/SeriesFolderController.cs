using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.Organizer;
using NzbDrone.Core.Books;
using Readarr.Http;

namespace Readarr.Api.V3.Series;

[V3ApiController("series")]
public class AuthorFolderController : Controller
{
    private readonly IAuthorService _seriesService;
    private readonly IBuildFileNames _fileNameBuilder;

    public SeriesFolderController(IAuthorService seriesService, IBuildFileNames fileNameBuilder)
    {
        _seriesService = seriesService;
        _fileNameBuilder = fileNameBuilder;
    }

    [HttpGet("{id}/folder")]
    public object GetFolder([FromRoute] int id)
    {
        var series = _seriesService.GetSeries(id);
        var folder = _fileNameBuilder.GetSeriesFolder(series);

        return new
        {
            folder
        };
    }
}
