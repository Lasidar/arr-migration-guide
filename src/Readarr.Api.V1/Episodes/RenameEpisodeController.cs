using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.MediaFiles;
using Readarr.Http;
using Readarr.Http.REST;

namespace Readarr.Api.V3.Episodes
{
    [V3ApiController("rename")]
    public class RenameEpisodeController : Controller
    {
        private readonly IRenameEditionFileService _renameEditionFileService;

        public RenameEpisodeController(IRenameEditionFileService renameEditionFileService)
        {
            _renameEditionFileService = renameEditionFileService;
        }

        [HttpGet]
        [Produces("application/json")]
        public List<RenameEpisodeResource> GetEpisodes(int seriesId, int? seasonNumber)
        {
            if (seasonNumber.HasValue)
            {
                return _renameEditionFileService.GetRenamePreviews(seriesId, seasonNumber.Value).ToResource();
            }

            return _renameEditionFileService.GetRenamePreviews(seriesId).ToResource();
        }

        [HttpGet("bulk")]
        [Produces("application/json")]
        public List<RenameEpisodeResource> GetEpisodes([FromQuery] List<int> seriesIds)
        {
            if (seriesIds is { Count: 0 })
            {
                throw new BadRequestException("seriesIds must be provided");
            }

            if (seriesIds.Any(seriesId => seriesId <= 0))
            {
                throw new BadRequestException("seriesIds must be positive integers");
            }

            return _renameEditionFileService.GetRenamePreviews(seriesIds).ToResource();
        }
    }
}
