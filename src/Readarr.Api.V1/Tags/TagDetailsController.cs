using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Tags;
using Readarr.Http;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Tags
{
    [V3ApiController("tag/detail")]
    public class TagDetailsController : RestController<TagDetailsResource>
    {
        private readonly ITagService _tagService;

        public TagDetailsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        protected override TagDetailsResource GetResourceById(int id)
        {
            return _tagService.Details(id).ToResource();
        }

        [HttpGet]
        [Produces("application/json")]
        public List<TagDetailsResource> GetAll()
        {
            return _tagService.Details().ToResource();
        }
    }
}
