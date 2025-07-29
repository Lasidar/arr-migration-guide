using System.Collections.Generic;
using System.Linq;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.DecisionEngine.Specifications
{
    public class SameEpisodesSpecification
    {
        private readonly IEditionService _episodeService;

        public SameEpisodesSpecification(IEditionService episodeService)
        {
            _episodeService = episodeService;
        }

        public bool IsSatisfiedBy(List<Episode> episodes)
        {
            var episodeIds = episodes.SelectList(e => e.Id);
            var episodeFileIds = episodes.Where(c => c.EditionFileId != 0).Select(c => c.EditionFileId).Distinct();

            foreach (var episodeFileId in episodeFileIds)
            {
                var episodesInFile = _episodeService.GetEpisodesByFileId(episodeFileId);

                if (episodesInFile.Select(e => e.Id).Except(episodeIds).Any())
                {
                    return false;
                }
            }

            return true;
        }
    }
}
