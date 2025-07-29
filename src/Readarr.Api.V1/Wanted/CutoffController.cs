using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.CustomFormats;
using Readarr.Core.Datastore;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.Tv;
using Readarr.SignalR;
using Readarr.Api.V3.Episodes;
using Readarr.Http;
using Readarr.Http.Extensions;

namespace Readarr.Api.V1.Wanted
{
    [V3ApiController("wanted/cutoff")]
    public class CutoffController : EpisodeControllerWithSignalR
    {
        private readonly IEpisodeCutoffService _episodeCutoffService;

        public CutoffController(IEpisodeCutoffService episodeCutoffService,
                            IEpisodeService episodeService,
                            ISeriesService seriesService,
                            IUpgradableSpecification upgradableSpecification,
                            ICustomFormatCalculationService formatCalculator,
                            IBroadcastSignalRMessage signalRBroadcaster)
            : base(episodeService, seriesService, upgradableSpecification, formatCalculator, signalRBroadcaster)
        {
            _episodeCutoffService = episodeCutoffService;
        }

        [HttpGet]
        [Produces("application/json")]
        public PagingResource<EpisodeResource> GetCutoffUnmetEpisodes([FromQuery] PagingRequestResource paging, bool includeSeries = false, bool includeEpisodeFile = false, bool includeImages = false, bool monitored = true)
        {
            var pagingResource = new PagingResource<EpisodeResource>(paging);
            var pagingSpec = pagingResource.MapToPagingSpec<EpisodeResource, Episode>(
                new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "episodes.airDateUtc",
                    "episodes.lastSearchTime",
                    "series.sortTitle"
                },
                "episodes.airDateUtc",
                SortDirection.Ascending);

            if (monitored)
            {
                pagingSpec.FilterExpressions.Add(v => v.Monitored == true && v.Series.Monitored == true);
            }
            else
            {
                pagingSpec.FilterExpressions.Add(v => v.Monitored == false || v.Series.Monitored == false);
            }

            var resource = pagingSpec.ApplyToPage(_episodeCutoffService.EpisodesWhereCutoffUnmet, v => MapToResource(v, includeSeries, includeEpisodeFile, includeImages));

            return resource;
        }
    }
}
