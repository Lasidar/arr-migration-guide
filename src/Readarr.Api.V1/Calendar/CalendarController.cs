using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Common.Extensions;
using Readarr.Core.CustomFormats;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.Tags;
using Readarr.Core.Tv;
using Readarr.SignalR;
using Readarr.Api.V3.Episodes;
using Readarr.Http;

namespace Readarr.Api.V1.Calendar
{
    [V3ApiController]
    public class CalendarController : EpisodeControllerWithSignalR
    {
        private readonly ITagService _tagService;

        public CalendarController(IBroadcastSignalRMessage signalR,
                            IEpisodeService episodeService,
                            ISeriesService seriesService,
                            IUpgradableSpecification qualityUpgradableSpecification,
                            ITagService tagService,
                            ICustomFormatCalculationService formatCalculator)
            : base(episodeService, seriesService, qualityUpgradableSpecification, formatCalculator, signalR)
        {
            _tagService = tagService;
        }

        [HttpGet]
        [Produces("application/json")]
        public List<EpisodeResource> GetCalendar(DateTime? start, DateTime? end, bool unmonitored = false, bool includeSeries = false, bool includeEpisodeFile = false, bool includeEpisodeImages = false, string tags = "")
        {
            var startUse = start ?? DateTime.Today;
            var endUse = end ?? DateTime.Today.AddDays(2);
            var episodes = _episodeService.EpisodesBetweenDates(startUse, endUse, unmonitored);
            var allSeries = _seriesService.GetAllSeries();
            var parsedTags = new List<int>();
            var result = new List<Episode>();

            if (tags.IsNotNullOrWhiteSpace())
            {
                parsedTags.AddRange(tags.Split(',').Select(_tagService.GetTag).Select(t => t.Id));
            }

            foreach (var episode in episodes)
            {
                var series = allSeries.SingleOrDefault(s => s.Id == episode.SeriesId);

                if (series == null)
                {
                    continue;
                }

                if (parsedTags.Any() && parsedTags.None(series.Tags.Contains))
                {
                    continue;
                }

                result.Add(episode);
            }

            var resources = MapToResource(result, includeSeries, includeEpisodeFile, includeEpisodeImages);

            return resources.OrderBy(e => e.AirDateUtc).ToList();
        }
    }
}
