using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.Messaging.Commands;
using Readarr.Http;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Author
{
    [V1ApiController("author/bulk")]
    public class AuthorBulkController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IManageCommandQueue _commandQueueManager;

        public AuthorBulkController(IAuthorService authorService, IManageCommandQueue commandQueueManager)
        {
            _authorService = authorService;
            _commandQueueManager = commandQueueManager;
        }

        [HttpPut("monitor")]
        public ActionResult<List<AuthorResource>> SetAuthorMonitored([FromBody] AuthorBulkResource resource)
        {
            var authors = _authorService.GetAuthorsByIds(resource.AuthorIds);

            foreach (var author in authors)
            {
                author.Monitored = resource.Monitored.Value;
            }

            _authorService.UpdateMany(authors);

            return Accepted(authors.ToResource());
        }

        [HttpPut("tags")]
        public ActionResult<List<AuthorResource>> UpdateAuthorTags([FromBody] AuthorBulkResource resource)
        {
            var authors = _authorService.GetAuthorsByIds(resource.AuthorIds);

            foreach (var author in authors)
            {
                if (resource.ApplyTags == ApplyTags.Add)
                {
                    resource.Tags.ForEach(t => author.Tags.Add(t));
                }
                else if (resource.ApplyTags == ApplyTags.Remove)
                {
                    resource.Tags.ForEach(t => author.Tags.Remove(t));
                }
                else
                {
                    author.Tags = new HashSet<int>(resource.Tags);
                }
            }

            _authorService.UpdateMany(authors);

            return Accepted(authors.ToResource());
        }

        [HttpPut("metadataprofile")]
        public ActionResult<List<AuthorResource>> UpdateMetadataProfile([FromBody] AuthorBulkResource resource)
        {
            var authors = _authorService.GetAuthorsByIds(resource.AuthorIds);

            foreach (var author in authors)
            {
                author.MetadataProfileId = resource.MetadataProfileId.Value;
            }

            _authorService.UpdateMany(authors);

            return Accepted(authors.ToResource());
        }

        [HttpPut("qualityprofile")]
        public ActionResult<List<AuthorResource>> UpdateQualityProfile([FromBody] AuthorBulkResource resource)
        {
            var authors = _authorService.GetAuthorsByIds(resource.AuthorIds);

            foreach (var author in authors)
            {
                author.QualityProfileId = resource.QualityProfileId.Value;
            }

            _authorService.UpdateMany(authors);

            return Accepted(authors.ToResource());
        }

        [HttpDelete]
        public ActionResult DeleteAuthors([FromBody] AuthorBulkResource resource)
        {
            var authors = _authorService.GetAuthorsByIds(resource.AuthorIds);

            foreach (var author in authors)
            {
                _authorService.DeleteAuthor(author.Id, resource.DeleteFiles ?? false, resource.AddImportListExclusion ?? false);
            }

            return NoContent();
        }

        [HttpPost("refresh")]
        public ActionResult RefreshAuthors([FromBody] AuthorBulkResource resource)
        {
            var command = new RefreshAuthorCommand
            {
                AuthorIds = resource.AuthorIds
            };

            _commandQueueManager.Push(command);

            return Accepted();
        }
    }

    public class AuthorBulkResource
    {
        public List<int> AuthorIds { get; set; }
        public bool? Monitored { get; set; }
        public int? QualityProfileId { get; set; }
        public int? MetadataProfileId { get; set; }
        public List<int> Tags { get; set; }
        public ApplyTags ApplyTags { get; set; }
        public bool? DeleteFiles { get; set; }
        public bool? AddImportListExclusion { get; set; }
    }

    public enum ApplyTags
    {
        Add,
        Remove,
        Replace
    }
}