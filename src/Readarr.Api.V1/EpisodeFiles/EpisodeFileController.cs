using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.CustomFormats;
using NzbDrone.Core.Datastore.Events;
using NzbDrone.Core.DecisionEngine.Specifications;
using NzbDrone.Core.Exceptions;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.MediaFiles.Events;
using NzbDrone.Core.Messaging.Events;
using NzbDrone.Core.Parser;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Books;
using NzbDrone.SignalR;
using Readarr.Http;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;
using BadRequestException = Sonarr.Http.REST.BadRequestException;

namespace Readarr.Api.V3.EditionFiles
{
    [V3ApiController]
    public class EditionFileController : RestControllerWithSignalR<EditionFileResource, EditionFile>,
                                 IHandle<EditionFileAddedEvent>,
                                 IHandle<EditionFileDeletedEvent>
    {
        private readonly IMediaFileService _mediaFileService;
        private readonly IDeleteMediaFiles _mediaFileDeletionService;
        private readonly IAuthorService _seriesService;
        private readonly ICustomFormatCalculationService _formatCalculator;
        private readonly IUpgradableSpecification _upgradableSpecification;

        public EditionFileController(IBroadcastSignalRMessage signalRBroadcaster,
                             IMediaFileService mediaFileService,
                             IDeleteMediaFiles mediaFileDeletionService,
                             IAuthorService seriesService,
                             ICustomFormatCalculationService formatCalculator,
                             IUpgradableSpecification upgradableSpecification)
            : base(signalRBroadcaster)
        {
            _mediaFileService = mediaFileService;
            _mediaFileDeletionService = mediaFileDeletionService;
            _seriesService = seriesService;
            _formatCalculator = formatCalculator;
            _upgradableSpecification = upgradableSpecification;
        }

        protected override EditionFileResource GetResourceById(int id)
        {
            var episodeFile = _mediaFileService.Get(id);
            var series = _seriesService.GetSeries(episodeFile.AuthorId);

            var resource = episodeFile.ToResource(series, _upgradableSpecification, _formatCalculator);

            return resource;
        }

        [HttpGet]
        [Produces("application/json")]
        public List<EditionFileResource> GetEditionFiles(int? seriesId, [FromQuery] List<int> episodeFileIds)
        {
            if (!seriesId.HasValue && !episodeFileIds.Any())
            {
                throw new BadRequestException("seriesId or episodeFileIds must be provided");
            }

            if (seriesId.HasValue)
            {
                var series = _seriesService.GetSeries(seriesId.Value);
                var files = _mediaFileService.GetFilesBySeries(seriesId.Value);

                if (files == null)
                {
                    return new List<EditionFileResource>();
                }

                return files.ConvertAll(e => e.ToResource(series, _upgradableSpecification, _formatCalculator))
                            .ToList();
            }
            else
            {
                var episodeFiles = _mediaFileService.Get(episodeFileIds);

                return episodeFiles.GroupBy(e => e.AuthorId)
                                   .SelectMany(f => f.ToList()
                                                     .ConvertAll(e => e.ToResource(_seriesService.GetSeries(f.Key), _upgradableSpecification, _formatCalculator)))
                                   .ToList();
            }
        }

        [RestPutById]
        [Consumes("application/json")]
        public ActionResult<EditionFileResource> SetQuality([FromBody] EditionFileResource episodeFileResource)
        {
            var episodeFile = _mediaFileService.Get(episodeFileResource.Id);
            episodeFile.Quality = episodeFileResource.Quality;

            if (episodeFileResource.SceneName != null && SceneChecker.IsSceneTitle(episodeFileResource.SceneName))
            {
                episodeFile.SceneName = episodeFileResource.SceneName;
            }

            if (episodeFileResource.ReleaseGroup != null)
            {
                episodeFile.ReleaseGroup = episodeFileResource.ReleaseGroup;
            }

            _mediaFileService.Update(episodeFile);
            return Accepted(episodeFile.Id);
        }

        [Obsolete("Use bulk endpoint instead")]
        [HttpPut("editor")]
        [Consumes("application/json")]
        public object SetQuality([FromBody] EditionFileListResource resource)
        {
            var episodeFiles = _mediaFileService.GetFiles(resource.EditionFileIds);

            foreach (var episodeFile in episodeFiles)
            {
                if (resource.Languages != null)
                {
                    episodeFile.Languages = resource.Languages;
                }

                if (resource.Quality != null)
                {
                    episodeFile.Quality = resource.Quality;
                }

                if (resource.SceneName != null && SceneChecker.IsSceneTitle(resource.SceneName))
                {
                    episodeFile.SceneName = resource.SceneName;
                }

                if (resource.ReleaseGroup != null)
                {
                    episodeFile.ReleaseGroup = resource.ReleaseGroup;
                }
            }

            _mediaFileService.Update(episodeFiles);

            var series = _seriesService.GetSeries(episodeFiles.First().AuthorId);

            return Accepted(episodeFiles.ConvertAll(f => f.ToResource(series, _upgradableSpecification, _formatCalculator)));
        }

        [RestDeleteById]
        public void DeleteEditionFile(int id)
        {
            var episodeFile = _mediaFileService.Get(id);

            if (episodeFile == null)
            {
                throw new NzbDroneClientException(HttpStatusCode.NotFound, "Episode file not found");
            }

            var series = _seriesService.GetSeries(episodeFile.AuthorId);

            _mediaFileDeletionService.DeleteEditionFile(series, episodeFile);
        }

        [HttpDelete("bulk")]
        [Consumes("application/json")]
        public object DeleteEditionFiles([FromBody] EditionFileListResource resource)
        {
            var episodeFiles = _mediaFileService.GetFiles(resource.EditionFileIds);
            var series = _seriesService.GetSeries(episodeFiles.First().AuthorId);

            foreach (var episodeFile in episodeFiles)
            {
                _mediaFileDeletionService.DeleteEditionFile(series, episodeFile);
            }

            return new { };
        }

        [HttpPut("bulk")]
        [Consumes("application/json")]
        public object SetPropertiesBulk([FromBody] List<EditionFileResource> resources)
        {
            var episodeFiles = _mediaFileService.GetFiles(resources.Select(r => r.Id));

            foreach (var episodeFile in episodeFiles)
            {
                var resourceEditionFile = resources.Single(r => r.Id == episodeFile.Id);

                if (resourceEditionFile.Languages != null)
                {
                    // Don't allow user to set files with 'Original' language
                    episodeFile.Languages = resourceEditionFile.Languages.Where(l => l != null && l != Language.Original).ToList();
                }

                if (resourceEditionFile.Quality != null)
                {
                    episodeFile.Quality = resourceEditionFile.Quality;
                }

                if (resourceEditionFile.SceneName != null && SceneChecker.IsSceneTitle(resourceEditionFile.SceneName))
                {
                    episodeFile.SceneName = resourceEditionFile.SceneName;
                }

                if (resourceEditionFile.ReleaseGroup != null)
                {
                    episodeFile.ReleaseGroup = resourceEditionFile.ReleaseGroup;
                }

                if (resourceEditionFile.IndexerFlags.HasValue)
                {
                    episodeFile.IndexerFlags = (IndexerFlags)resourceEditionFile.IndexerFlags;
                }

                if (resourceEditionFile.ReleaseType != null)
                {
                    episodeFile.ReleaseType = (ReleaseType)resourceEditionFile.ReleaseType;
                }
            }

            _mediaFileService.Update(episodeFiles);

            var series = _seriesService.GetSeries(episodeFiles.First().AuthorId);

            return Accepted(episodeFiles.ConvertAll(f => f.ToResource(series, _upgradableSpecification, _formatCalculator)));
        }

        [NonAction]
        public void Handle(EditionFileAddedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, message.EditionFile.Id);
        }

        [NonAction]
        public void Handle(EditionFileDeletedEvent message)
        {
            BroadcastResourceChange(ModelAction.Deleted, message.EditionFile.Id);
        }
    }
}
