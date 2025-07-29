using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Books.Commands;
using Readarr.Core.Books.Events;
using Readarr.Core.Datastore;
using Readarr.Core.Datastore.Events;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.RootFolders;
using Readarr.Core.AuthorStats;
using Readarr.Core.Validation;
using Readarr.Core.Validation.Paths;
using Readarr.Http;
using Readarr.Http.Extensions;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;
using Readarr.SignalR;

namespace Readarr.Api.V1.Author
{
    [V3ApiController]
    public class AuthorController : RestControllerWithSignalR<AuthorResource, Core.Books.Author>,
                                IHandle<BookFileDeletedEvent>,
                                IHandle<AuthorUpdatedEvent>,
                                IHandle<AuthorEditedEvent>,
                                IHandle<AuthorDeletedEvent>,
                                IHandle<AuthorRenamedEvent>,
                                IHandle<MediaCoversUpdatedEvent>
    {
        private readonly IAuthorService _authorService;
        private readonly IAddAuthorService _addAuthorService;
        private readonly IAuthorStatisticsService _authorStatisticsService;
        private readonly IMapCoversToLocal _coverMapper;
        private readonly IManageCommandQueue _commandQueueManager;
        private readonly IRootFolderService _rootFolderService;

        public AuthorController(IBroadcastSignalRMessage signalRBroadcaster,
                            IAuthorService authorService,
                            IAddAuthorService addAuthorService,
                            IAuthorStatisticsService authorStatisticsService,
                            IMapCoversToLocal coverMapper,
                            IManageCommandQueue commandQueueManager,
                            IRootFolderService rootFolderService,
                            RootFolderValidator rootFolderValidator,
                            MappedNetworkDriveValidator mappedNetworkDriveValidator,
                            AuthorPathValidator authorPathValidator,
                            AuthorExistsValidator authorExistsValidator,
                            AuthorAncestorValidator authorAncestorValidator,
                            SystemFolderValidator systemFolderValidator,
                            QualityProfileExistsValidator qualityProfileExistsValidator,
                            MetadataProfileExistsValidator metadataProfileExistsValidator)
            : base(signalRBroadcaster)
        {
            _authorService = authorService;
            _addAuthorService = addAuthorService;
            _authorStatisticsService = authorStatisticsService;
            _coverMapper = coverMapper;
            _commandQueueManager = commandQueueManager;
            _rootFolderService = rootFolderService;

            Http.Validation.RuleBuilderExtensions.ValidId(SharedValidator.RuleFor(s => s.QualityProfileId));
            Http.Validation.RuleBuilderExtensions.ValidId(SharedValidator.RuleFor(s => s.MetadataProfileId));

            SharedValidator.RuleFor(s => s.Path)
                          .Cascade(CascadeMode.Stop)
                          .IsValidPath()
                          .SetValidator(rootFolderValidator)
                          .SetValidator(mappedNetworkDriveValidator)
                          .SetValidator(authorPathValidator)
                          .SetValidator(authorAncestorValidator)
                          .SetValidator(systemFolderValidator)
                          .When(s => !s.Path.IsNullOrWhiteSpace());

            SharedValidator.RuleFor(s => s.QualityProfileId).SetValidator(qualityProfileExistsValidator);
            SharedValidator.RuleFor(s => s.MetadataProfileId).SetValidator(metadataProfileExistsValidator);

            PostValidator.RuleFor(s => s.Path).IsValidPath().When(s => s.RootFolderPath.IsNullOrWhiteSpace());
            PostValidator.RuleFor(s => s.RootFolderPath).IsValidPath().When(s => s.Path.IsNullOrWhiteSpace());
            PostValidator.RuleFor(s => s.ForeignAuthorId).NotEmpty().SetValidator(authorExistsValidator);

            PutValidator.RuleFor(s => s.Path).IsValidPath();
        }

        protected override AuthorResource GetResourceById(int id)
        {
            var author = _authorService.GetAuthor(id);
            return GetAuthorResource(author);
        }

        [HttpGet]
        public List<AuthorResource> AllAuthors()
        {
            var authorStats = _authorStatisticsService.AuthorStatistics();
            var authorResources = _authorService.GetAllAuthors().ToResource();

            MapCoversToLocal(authorResources.ToArray());
            LinkAuthorStatistics(authorResources, authorStats);

            return authorResources;
        }

        [RestPostById]
        public ActionResult<AuthorResource> AddAuthor([FromBody] AuthorResource authorResource)
        {
            var author = _addAuthorService.AddAuthor(authorResource.ToModel());

            return Created(author.Id);
        }

        [RestPutById]
        public ActionResult<AuthorResource> UpdateAuthor([FromBody] AuthorResource authorResource, [FromQuery] bool moveFiles = false)
        {
            var author = _authorService.GetAuthor(authorResource.Id);

            if (moveFiles)
            {
                var sourcePath = author.Path;
                var destinationPath = authorResource.Path;

                _commandQueueManager.Push(new MoveAuthorCommand
                {
                    AuthorId = author.Id,
                    SourcePath = sourcePath,
                    DestinationPath = destinationPath,
                    Trigger = CommandTrigger.Manual
                });
            }

            var model = authorResource.ToModel();
            model.Id = author.Id;

            _authorService.UpdateAuthor(model);

            BroadcastResourceChange(ModelAction.Updated, authorResource);

            return Accepted(authorResource);
        }

        [RestDeleteById]
        public void DeleteAuthor(int id, [FromQuery] bool deleteFiles = false, [FromQuery] bool addImportListExclusion = false)
        {
            _authorService.DeleteAuthor(new List<int> { id }, deleteFiles, addImportListExclusion);
        }

        private AuthorResource GetAuthorResource(Core.Books.Author author)
        {
            if (author == null)
            {
                return null;
            }

            var resource = author.ToResource();
            MapCoversToLocal(resource);

            return resource;
        }

        private void MapCoversToLocal(params AuthorResource[] authors)
        {
            foreach (var authorResource in authors)
            {
                _coverMapper.ConvertToLocalUrls(authorResource.Id, MediaCoverEntity.Author, authorResource.Images);
            }
        }

        private void LinkAuthorStatistics(List<AuthorResource> resources, List<AuthorStatistics> authorStatistics)
        {
            foreach (var author in resources)
            {
                var stats = authorStatistics.SingleOrDefault(ss => ss.AuthorId == author.Id);
                author.Statistics = stats?.ToResource() ?? new AuthorStatisticsResource();
            }
        }

        [NonAction]
        public void Handle(BookFileDeletedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, _authorService.GetAuthor(message.BookFile.Author.Value.Id).ToResource());
        }

        [NonAction]
        public void Handle(AuthorUpdatedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, message.Author.ToResource());
        }

        [NonAction]
        public void Handle(AuthorEditedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, message.Author.ToResource());
        }

        [NonAction]
        public void Handle(AuthorDeletedEvent message)
        {
            BroadcastResourceChange(ModelAction.Deleted, message.Author.ToResource());
        }

        [NonAction]
        public void Handle(AuthorRenamedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, message.Author.ToResource());
        }

        [NonAction]
        public void Handle(MediaCoversUpdatedEvent message)
        {
            if (message.Updated)
            {
                BroadcastResourceChange(ModelAction.Updated, _authorService.GetAuthor(message.Author.Id).ToResource());
            }
        }
    }
}