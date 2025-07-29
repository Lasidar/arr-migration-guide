using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.CustomFormats;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.DecisionEngine.Specifications;
using NzbDrone.Core.Download;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Books;
using NzbDrone.SignalR;
using Readarr.Api.V3.EditionFiles;
using Readarr.Api.V3.Series;
using Readarr.Http.REST;

namespace Readarr.Api.V3.Episodes
{
    public abstract class EditionControllerWithSignalR : RestControllerWithSignalR<EpisodeResource, Episode>,
                                                         IHandle<EpisodeGrabbedEvent>,
                                                         IHandle<EpisodeImportedEvent>,
                                                         IHandle<EditionFileDeletedEvent>
    {
        protected readonly IEditionService _episodeService;
        protected readonly IAuthorService _seriesService;
        protected readonly IUpgradableSpecification _upgradableSpecification;
        protected readonly ICustomFormatCalculationService _formatCalculator;

        protected EpisodeControllerWithSignalR(IEditionService episodeService,
                                           IAuthorService seriesService,
                                           IUpgradableSpecification upgradableSpecification,
                                           ICustomFormatCalculationService formatCalculator,
                                           IBroadcastSignalRMessage signalRBroadcaster)
            : base(signalRBroadcaster)
        {
            _episodeService = episodeService;
            _seriesService = seriesService;
            _upgradableSpecification = upgradableSpecification;
            _formatCalculator = formatCalculator;
        }

        protected EpisodeControllerWithSignalR(IEditionService episodeService,
                                           IAuthorService seriesService,
                                           IUpgradableSpecification upgradableSpecification,
                                           ICustomFormatCalculationService formatCalculator,
                                           IBroadcastSignalRMessage signalRBroadcaster,
                                           string resource)
            : base(signalRBroadcaster)
        {
            _episodeService = episodeService;
            _seriesService = seriesService;
            _upgradableSpecification = upgradableSpecification;
            _formatCalculator = formatCalculator;
        }

        protected override EpisodeResource GetResourceById(int id)
        {
            var episode = _episodeService.GetEpisode(id);
            var resource = MapToResource(episode, true, true, true);
            return resource;
        }

        protected EpisodeResource MapToResource(Episode episode, bool includeSeries, bool includeEditionFile, bool includeImages)
        {
            var resource = episode.ToResource();

            if (includeSeries || includeEditionFile || includeImages)
            {
                var series = episode.Series ?? _seriesService.GetSeries(episode.AuthorId);

                if (includeSeries)
                {
                    resource.Series = series.ToResource();
                }

                if (includeEditionFile && episode.EditionFileId != 0)
                {
                    resource.EditionFile = episode.EditionFile.Value.ToResource(series, _upgradableSpecification, _formatCalculator);
                }

                if (includeImages)
                {
                    resource.Images = episode.Images;
                }
            }

            return resource;
        }

        protected List<EpisodeResource> MapToResource(List<Episode> episodes, bool includeSeries, bool includeEditionFile, bool includeImages)
        {
            var result = episodes.ToResource();

            if (includeSeries || includeEditionFile || includeImages)
            {
                var seriesDict = new Dictionary<int, NzbDrone.Core.Tv.Series>();
                for (var i = 0; i < episodes.Count; i++)
                {
                    var episode = episodes[i];
                    var resource = result[i];

                    var series = episode.Series ?? seriesDict.GetValueOrDefault(episodes[i].AuthorId) ?? _seriesService.GetSeries(episodes[i].AuthorId);
                    seriesDict[series.Id] = series;

                    if (includeSeries)
                    {
                        resource.Series = series.ToResource();
                    }

                    if (includeEditionFile && episode.EditionFileId != 0)
                    {
                        resource.EditionFile = episode.EditionFile.Value.ToResource(series, _upgradableSpecification, _formatCalculator);
                    }

                    if (includeImages)
                    {
                        resource.Images = episode.Images;
                    }
                }
            }

            return result;
        }

        [NonAction]
        public void Handle(EpisodeGrabbedEvent message)
        {
            foreach (var episode in message.Episode.Episodes)
            {
                var resource = episode.ToResource();
                resource.Grabbed = true;

                BroadcastResourceChange(ModelAction.Updated, resource);
            }
        }

        [NonAction]
        public void Handle(EpisodeImportedEvent message)
        {
            foreach (var episode in message.EpisodeInfo.Episodes)
            {
                BroadcastResourceChange(ModelAction.Updated, episode.Id);
            }
        }

        [NonAction]
        public void Handle(EditionFileDeletedEvent message)
        {
            foreach (var episode in message.EditionFile.Episodes.Value)
            {
                BroadcastResourceChange(ModelAction.Updated, episode.Id);
            }
        }
    }
}
