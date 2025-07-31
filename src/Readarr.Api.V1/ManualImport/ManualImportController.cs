using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.MediaFiles.BookImport.Manual;
using Readarr.Core.Qualities;
using Readarr.Http;

namespace Readarr.Api.V1.ManualImport
{
    [V1ApiController("manualimport")]
    public class ManualImportController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly IManualImportService _manualImportService;

        public ManualImportController(IAuthorService authorService,
                                     IBookService bookService,
                                     IManualImportService manualImportService)
        {
            _authorService = authorService;
            _bookService = bookService;
            _manualImportService = manualImportService;
        }

        [HttpGet]
        public List<ManualImportResource> GetMediaFiles([FromQuery] string folder,
                                                       [FromQuery] string downloadId,
                                                       [FromQuery] int? authorId,
                                                       [FromQuery] bool filterExistingFiles = true)
        {
            if (string.IsNullOrWhiteSpace(folder) && string.IsNullOrWhiteSpace(downloadId))
            {
                throw new BadRequestException("Either folder or downloadId must be provided");
            }

            var filter = filterExistingFiles ? FilterFilesType.Matched : FilterFilesType.None;

            return _manualImportService.GetMediaFiles(folder, downloadId, authorId, filter)
                                      .Select(f => f.ToResource())
                                      .ToList();
        }

        [HttpPost]
        public IActionResult UpdateItems([FromBody] List<ManualImportUpdateResource> items)
        {
            var updateItems = new List<ManualImportItem>();

            foreach (var item in items)
            {
                var manualImportItem = new ManualImportItem
                {
                    Id = item.Id,
                    Path = item.Path,
                    Name = item.Name,
                    Author = item.AuthorId.HasValue ? _authorService.GetAuthor(item.AuthorId.Value) : null,
                    Book = item.BookId.HasValue ? _bookService.GetBook(item.BookId.Value) : null,
                    Quality = item.Quality,
                    ReleaseGroup = item.ReleaseGroup,
                    Languages = item.Languages,
                    DownloadId = item.DownloadId,
                    AdditionalFile = item.AdditionalFile,
                    ReplaceExistingFiles = item.ReplaceExistingFiles,
                    DisableReleaseSwitching = item.DisableReleaseSwitching
                };

                updateItems.Add(manualImportItem);
            }

            return Ok(_manualImportService.UpdateItems(updateItems).Select(x => x.ToResource()));
        }
    }
}
