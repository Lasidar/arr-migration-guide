using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.Books.Commands;
using Readarr.Core.Datastore.Events;
using Readarr.Core.MediaCover;
using Readarr.Core.MediaFiles;
using Readarr.Core.MediaFiles.Events;
using Readarr.Core.Messaging.Commands;
using Readarr.Core.Messaging.Events;
using Readarr.Core.RootFolders;
using Readarr.Core.Validation;
using Readarr.Core.Validation.Paths;
using Readarr.Http;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;
using Readarr.SignalR;

namespace Readarr.Api.V1.Author
{
    [V1ApiController]
    public class AuthorController : RestControllerWithSignalR<AuthorResource, Core.Books.Author>,
                                IHandle<BookImportedEvent>,
                                IHandle<BookEditedEvent>,
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
                           .Cascade(CascadeMode.StopOnFirstFailure)
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
            PostValidator.RuleFor(s => s.AuthorName).NotEmpty();
            PostValidator.RuleFor(s => s.ForeignAuthorId).NotEmpty().SetValidator(authorExistsValidator);

            PutValidator.RuleFor(s => s.Path).IsValidPath();
        }

        protected override AuthorResource GetResourceById(int id)
        {
            var author = _authorService.GetAuthor(id);
            return GetAuthorResource(author);
        }

        [HttpGet]
        public List<AuthorResource> GetAuthors()
        {
            var authors = _authorService.GetAllAuthors();
            var resources = MapToResource(authors);
            LinkAuthorStatistics(resources, authors);
            return resources;
        }

        [RestPostById]
        public ActionResult<AuthorResource> AddAuthor(AuthorResource authorResource)
        {
            var author = _addAuthorService.AddAuthor(authorResource.ToModel());
            return Created(GetAuthorResource(author));
        }

        [RestPutById]
        public ActionResult<AuthorResource> UpdateAuthor(AuthorResource authorResource, [FromQuery] bool moveFiles = false)
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

            var model = authorResource.ToModel(author);
            _authorService.UpdateAuthor(model);

            BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(model));

            return Accepted(authorResource.Id);
        }

        [RestDeleteById]
        public void DeleteAuthor(int id, [FromQuery] bool deleteFiles = false, [FromQuery] bool addImportListExclusion = false)
        {
            _authorService.DeleteAuthor(id, deleteFiles, addImportListExclusion);
        }

        private AuthorResource GetAuthorResource(Core.Books.Author author)
        {
            if (author == null) return null;

            var resource = author.ToResource();
            MapCoversToLocal(resource);
            FetchAndLinkAuthorStatistics(resource);
            LinkRootFolderPath(resource);

            return resource;
        }

        private List<AuthorResource> MapToResource(IEnumerable<Core.Books.Author> authors)
        {
            var resources = authors.Select(author => author.ToResource()).ToList();
            
            foreach (var resource in resources)
            {
                MapCoversToLocal(resource);
                LinkRootFolderPath(resource);
            }

            return resources;
        }

        private void MapCoversToLocal(AuthorResource resource)
        {
            _coverMapper.ConvertToLocalUrls(resource.Id, MediaCoverEntity.Author, resource.Images);
        }

        private void LinkRootFolderPath(AuthorResource resource)
        {
            resource.RootFolderPath = _rootFolderService.GetBestRootFolderPath(resource.Path);
        }

        private void FetchAndLinkAuthorStatistics(AuthorResource resource)
        {
            LinkAuthorStatistics(resource, _authorStatisticsService.AuthorStatistics(resource.Id));
        }

        private void LinkAuthorStatistics(List<AuthorResource> resources, List<Core.Books.Author> authors)
        {
            var authorStatistics = _authorStatisticsService.AuthorStatistics();
            
            foreach (var author in resources)
            {
                var stats = authorStatistics.SingleOrDefault(ss => ss.AuthorId == author.Id);
                LinkAuthorStatistics(author, stats);
            }
        }

        private void LinkAuthorStatistics(AuthorResource resource, AuthorStatistics authorStatistics)
        {
            if (authorStatistics?.BookStatistics != null)
            {
                resource.Statistics = new AuthorStatisticsResource
                {
                    BookCount = authorStatistics.BookStatistics.Count(v => v.BookFileCount > 0),
                    BookFileCount = authorStatistics.BookStatistics.Sum(v => v.BookFileCount),
                    TotalBookCount = authorStatistics.BookStatistics.Count,
                    SizeOnDisk = authorStatistics.SizeOnDisk,
                    PercentOfBooks = 0
                };

                if (resource.Statistics.TotalBookCount > 0)
                {
                    resource.Statistics.PercentOfBooks = (decimal)resource.Statistics.BookCount / resource.Statistics.TotalBookCount * 100;
                }
            }
        }

        public void Handle(BookImportedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(message.Author));
        }

        public void Handle(BookEditedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(message.Book.Author.Value));
        }

        public void Handle(BookFileDeletedEvent message)
        {
            if (message.Reason == DeleteMediaFileReason.Upgrade)
            {
                return;
            }

            BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(message.BookFile.Author.Value));
        }

        public void Handle(AuthorUpdatedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(message.Author));
        }

        public void Handle(AuthorEditedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(message.Author));
        }

        public void Handle(AuthorDeletedEvent message)
        {
            BroadcastResourceChange(ModelAction.Deleted, message.Author.ToResource());
        }

        public void Handle(AuthorRenamedEvent message)
        {
            BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(message.Author));
        }

        public void Handle(MediaCoversUpdatedEvent message)
        {
            if (message.Updated)
            {
                BroadcastResourceChange(ModelAction.Updated, GetAuthorResource(message.Author));
            }
        }
    }
}