using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Core.MetadataSource;
using Readarr.Http;

namespace Readarr.Api.V1.Books
{
    [V1ApiController("book/lookup")]
    public class BookLookupController : Controller
    {
        private readonly ISearchForNewBook _searchProxy;
        private readonly IMapCoversToLocal _coverMapper;

        public BookLookupController(ISearchForNewBook searchProxy, IMapCoversToLocal coverMapper)
        {
            _searchProxy = searchProxy;
            _coverMapper = coverMapper;
        }

        [HttpGet]
        public List<BookResource> GetBooksByTerm([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return new List<BookResource>();
            }

            var searchResults = new List<Book>();

            // Try searching by ISBN first
            if (IsValidIsbn(term))
            {
                var isbnResults = _searchProxy.SearchByIsbn(term);
                searchResults.AddRange(isbnResults);
            }

            // If no ISBN results, try general search
            if (!searchResults.Any())
            {
                var generalResults = _searchProxy.SearchForNewBook(term, null);
                searchResults.AddRange(generalResults);
            }

            return MapToResource(searchResults);
        }

        [HttpGet("isbn/{isbn}")]
        public ActionResult<BookResource> GetBookByIsbn(string isbn)
        {
            if (!IsValidIsbn(isbn))
            {
                return BadRequest("Invalid ISBN format");
            }

            var searchResults = _searchProxy.SearchByIsbn(isbn);
            var book = searchResults.FirstOrDefault();

            if (book == null)
            {
                return NotFound();
            }

            return MapToResource(new[] { book }).FirstOrDefault();
        }

        [HttpGet("goodreads/{goodreadsId}")]
        public ActionResult<BookResource> GetBookByGoodreadsId(string goodreadsId)
        {
            var searchResults = _searchProxy.SearchByGoodreadsId(goodreadsId);
            var book = searchResults.FirstOrDefault();

            if (book == null)
            {
                return NotFound();
            }

            return MapToResource(new[] { book }).FirstOrDefault();
        }

        private List<BookResource> MapToResource(IEnumerable<Book> books)
        {
            var resources = new List<BookResource>();

            foreach (var book in books)
            {
                var resource = book.ToResource();
                
                // Ensure we have author information
                if (string.IsNullOrEmpty(resource.AuthorName) && book.Metadata.Value != null)
                {
                    resource.AuthorName = book.Metadata.Value.AuthorName;
                }

                // Convert cover URLs to local
                _coverMapper.ConvertToLocalUrls(0, MediaCoverEntity.Book, resource.Images);

                resources.Add(resource);
            }

            return resources;
        }

        private bool IsValidIsbn(string isbn)
        {
            if (string.IsNullOrWhiteSpace(isbn))
            {
                return false;
            }

            var cleanIsbn = isbn.Replace("-", "").Replace(" ", "");
            return (cleanIsbn.Length == 10 || cleanIsbn.Length == 13) && cleanIsbn.All(char.IsDigit);
        }
    }
}