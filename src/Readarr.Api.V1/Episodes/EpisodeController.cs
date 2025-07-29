using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.CustomFormats;
using NzbDrone.Core.DecisionEngine.Specifications;
using NzbDrone.Core.Books;
using NzbDrone.SignalR;
using Readarr.Http;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;

namespace Readarr.Api.V3.Episodes
{
    [V3ApiController]
    public class EditionController : EpisodeControllerWithSignalR
    {
        public EpisodeController(IAuthorService seriesService,
                             IEditionService episodeService,
                             IUpgradableSpecification upgradableSpecification,
                             ICustomFormatCalculationService formatCalculator,
                             IBroadcastSignalRMessage signalRBroadcaster)
            : base(episodeService, seriesService, upgradableSpecification, formatCalculator, signalRBroadcaster)
        {
        }

        [HttpGet]
        [Produces("application/json")]
        public List<EpisodeResource> GetEpisodes(int? seriesId, int? seasonNumber, [FromQuery]List<int> episodeIds, int? episodeFileId, bool includeSeries = false, bool includeEditionFile = false, bool includeImages = false)
        {
            if (seriesId.HasValue)
            {
                if (seasonNumber.HasValue)
                {
                    return MapToResource(_episodeService.GetEpisodesBySeason(seriesId.Value, seasonNumber.Value), includeSeries, includeEditionFile, includeImages);
                }

                return MapToResource(_episodeService.GetEpisodeBySeries(seriesId.Value), includeSeries, includeEditionFile, includeImages);
            }
            else if (episodeIds.Any())
            {
                return MapToResource(_episodeService.GetEpisodes(episodeIds), includeSeries, includeEditionFile, includeImages);
            }
            else if (episodeFileId.HasValue)
            {
                return MapToResource(_episodeService.GetEpisodesByFileId(episodeFileId.Value), includeSeries, includeEditionFile, includeImages);
            }

            throw new BadRequestException("seriesId or episodeIds must be provided");
        }

        [RestPutById]
        [Consumes("application/json")]
        public ActionResult<EpisodeResource> SetEpisodeMonitored([FromRoute] int id, [FromBody] EpisodeResource resource)
        {
            _episodeService.SetEpisodeMonitored(id, resource.Monitored);

            resource = MapToResource(_episodeService.GetEpisode(id), false, false, false);

            return Accepted(resource);
        }

        [HttpPut("monitor")]
        [Consumes("application/json")]
        public IActionResult SetEpisodesMonitored([FromBody] EpisodesMonitoredResource resource, [FromQuery] bool includeImages = false)
        {
            if (resource.EpisodeIds.Count == 1)
            {
                _episodeService.SetEpisodeMonitored(resource.EpisodeIds.First(), resource.Monitored);
            }
            else
            {
                _episodeService.SetMonitored(resource.EpisodeIds, resource.Monitored);
            }

            var resources = MapToResource(_episodeService.GetEpisodes(resource.EpisodeIds), false, false, includeImages);

            return Accepted(resources);
        }
    }
}
