using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Api.V1.Author;
using Readarr.Api.V1.Book;
using Readarr.Core.Books;
using Readarr.Http;

namespace Readarr.Api.V1.Calendar
{
    [V1ApiController("calendar")]
    public class BookCalendarController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;

        public BookCalendarController(IBookService bookService, IAuthorService authorService)
        {
            _bookService = bookService;
            _authorService = authorService;
        }

        [HttpGet]
        public List<BookResource> GetCalendar([FromQuery] DateTime? start, [FromQuery] DateTime? end, [FromQuery] bool unmonitored = false, [FromQuery] bool includeAuthor = false)
        {
            var startDate = start ?? DateTime.Today;
            var endDate = end ?? DateTime.Today.AddDays(14);

            var books = _bookService.GetBooksBetweenDates(startDate, endDate, unmonitored);
            
            var resources = books.ToResource();

            if (includeAuthor)
            {
                var authorDict = _authorService.GetAuthorsByIds(books.Select(b => b.AuthorMetadataId).Distinct().ToList())
                    .ToDictionary(a => a.AuthorMetadataId);

                foreach (var book in resources)
                {
                    if (authorDict.TryGetValue(book.AuthorMetadataId, out var author))
                    {
                        book.Author = author.ToResource();
                    }
                }
            }

            return resources;
        }
    }
}