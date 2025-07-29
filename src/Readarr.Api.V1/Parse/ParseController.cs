using Microsoft.AspNetCore.Mvc;
using Readarr.Common.Extensions;
using Readarr.Core.CustomFormats;
using Readarr.Core.Download.Aggregation;
using Readarr.Core.Parser;
using Readarr.Api.V3.CustomFormats;
using Readarr.Api.V3.Episodes;
using Readarr.Api.V3.Series;
using Readarr.Http;

namespace Readarr.Api.V1.Parse
{
    [V3ApiController]
    public class ParseController : Controller
    {
        private readonly IParsingService _parsingService;
        private readonly IRemoteEpisodeAggregationService _aggregationService;
        private readonly ICustomFormatCalculationService _formatCalculator;

        public ParseController(IParsingService parsingService,
                               IRemoteEpisodeAggregationService aggregationService,
                               ICustomFormatCalculationService formatCalculator)
        {
            _parsingService = parsingService;
            _aggregationService = aggregationService;
            _formatCalculator = formatCalculator;
        }

        [HttpGet]
        [Produces("application/json")]
        public ParseResource Parse(string title, string path)
        {
            if (title.IsNullOrWhiteSpace())
            {
                return null;
            }

            var parsedEpisodeInfo = path.IsNotNullOrWhiteSpace() ? Parser.ParsePath(path) : Parser.ParseTitle(title);

            if (parsedEpisodeInfo == null)
            {
                return new ParseResource
                {
                    Title = title
                };
            }

            var remoteEpisode = _parsingService.Map(parsedEpisodeInfo, 0, 0, null);

            if (remoteEpisode != null)
            {
                _aggregationService.Augment(remoteEpisode);

                remoteEpisode.CustomFormats = _formatCalculator.ParseCustomFormat(remoteEpisode, 0);
                remoteEpisode.CustomFormatScore = remoteEpisode?.Series?.QualityProfile?.Value.CalculateCustomFormatScore(remoteEpisode.CustomFormats) ?? 0;

                return new ParseResource
                {
                    Title = title,
                    ParsedEpisodeInfo = remoteEpisode.ParsedEpisodeInfo,
                    Series = remoteEpisode.Series.ToResource(),
                    Episodes = remoteEpisode.Episodes.ToResource(),
                    Languages = remoteEpisode.Languages,
                    CustomFormats = remoteEpisode.CustomFormats?.ToResource(false),
                    CustomFormatScore = remoteEpisode.CustomFormatScore
                };
            }
            else
            {
                return new ParseResource
                {
                    Title = title,
                    ParsedEpisodeInfo = parsedEpisodeInfo
                };
            }
        }
    }
}
