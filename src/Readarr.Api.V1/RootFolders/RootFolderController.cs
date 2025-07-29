using System.Collections.Generic;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NzbDrone.Core.RootFolders;
using NzbDrone.Core.Validation.Paths;
using NzbDrone.SignalR;
using Readarr.Http;
using Readarr.Http.Extensions;
using Readarr.Http.REST;
using Readarr.Http.REST.Attributes;

namespace Readarr.Api.V3.RootFolders
{
    [V3ApiController]
    public class RootFolderController : RestControllerWithSignalR<RootFolderResource, RootFolder>
    {
        private readonly IRootFolderService _rootFolderService;

        public RootFolderController(IRootFolderService rootFolderService,
                                IBroadcastSignalRMessage signalRBroadcaster,
                                RootFolderValidator rootFolderValidator,
                                PathExistsValidator pathExistsValidator,
                                MappedNetworkDriveValidator mappedNetworkDriveValidator,
                                RecycleBinValidator recycleBinValidator,
                                StartupFolderValidator startupFolderValidator,
                                SystemFolderValidator systemFolderValidator,
                                FolderWritableValidator folderWritableValidator)
        : base(signalRBroadcaster)
        {
            _rootFolderService = rootFolderService;

            SharedValidator.RuleFor(c => c.Path)
                .Cascade(CascadeMode.Stop)
                .IsValidPath()
                           .SetValidator(rootFolderValidator)
                           .SetValidator(mappedNetworkDriveValidator)
                           .SetValidator(startupFolderValidator)
                           .SetValidator(recycleBinValidator)
                           .SetValidator(pathExistsValidator)
                           .SetValidator(systemFolderValidator)
                           .SetValidator(folderWritableValidator);
        }

        protected override RootFolderResource GetResourceById(int id)
        {
            var timeout = Request?.GetBooleanQueryParameter("timeout", true) ?? true;

            return _rootFolderService.Get(id, timeout).ToResource();
        }

        [RestPostById]
        [Consumes("application/json")]
        public ActionResult<RootFolderResource> CreateRootFolder([FromBody] RootFolderResource rootFolderResource)
        {
            var model = rootFolderResource.ToModel();

            return Created(_rootFolderService.Add(model).Id);
        }

        [HttpGet]
        [Produces("application/json")]
        public List<RootFolderResource> GetRootFolders()
        {
            return _rootFolderService.AllWithUnmappedFolders().ToResource();
        }

        [RestDeleteById]
        public void DeleteFolder(int id)
        {
            _rootFolderService.Remove(id);
        }
    }
}
