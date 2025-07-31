using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Readarr.Core.Books;
using Readarr.Core.MediaCover;
using Readarr.Core.MetadataSource;
using Readarr.Http;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Search
{
    [V1ApiController("search")]
    public class BookSearchController : RestController<BookSearchResource>
    {
        private readonly ISearchForNewBook _searchProxy;
        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IAddBookService _addBookService;
        private readonly IMapCoversToLocal _coverMapper;

        public BookSearchController(ISearchForNewBook searchProxy,
                                   IBookService bookService,
                                   IAuthorService authorService,
                                   IAddBookService addBookService,
                                   IMapCoversToLocal coverMapper)
        {
            _searchProxy = searchProxy;
            _bookService = bookService;
            _authorService = authorService;
            _addBookService = addBookService;
            _coverMapper = coverMapper;
        }

        protected override BookSearchResource GetResourceById(int id)
        {
            throw new System.NotImplementedException();
        }

        [HttpGet]
        public List<BookSearchResource> SearchBooks([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return new List<BookSearchResource>();
            }

            var searchResults = new List<Book>();
            
            // Search by ISBN
            if (IsIsbn(term))
            {
                var isbnResults = _searchProxy.SearchByIsbn(term);
                searchResults.AddRange(isbnResults);
            }
            // Search by ASIN
            else if (IsAsin(term))
            {
                var asinResults = _searchProxy.SearchByAsin(term);
                searchResults.AddRange(asinResults);
            }
            // Search by Goodreads ID
            else if (IsGoodreadsId(term))
            {
                var goodreadsResults = _searchProxy.SearchByGoodreadsId(term);
                searchResults.AddRange(goodreadsResults);
            }
            // General search
            else
            {
                var generalResults = _searchProxy.SearchForNewBook(term, null);
                searchResults.AddRange(generalResults);
            }

            return MapToResource(searchResults);
        }

        [HttpGet("author")]
        public List<AuthorSearchResource> SearchAuthors([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return new List<AuthorSearchResource>();
            }

            var searchResults = _searchProxy.SearchForNewAuthor(term);
            
            return searchResults.Select(author => new AuthorSearchResource
            {
                ForeignAuthorId = author.Metadata.Value.ForeignAuthorId,
                Name = author.Name,
                Overview = author.Metadata.Value.Overview,
                Images = author.Metadata.Value.Images,
                Status = author.Metadata.Value.Status,
                Ratings = author.Metadata.Value.Ratings,
                Genres = author.Metadata.Value.Genres
            }).ToList();
        }

        private List<BookSearchResource> MapToResource(IEnumerable<Book> books)
        {
            var resources = new List<BookSearchResource>();

            foreach (var book in books)
            {
                var resource = new BookSearchResource
                {
                    ForeignBookId = book.ForeignBookId,
                    Title = book.Title,
                    ReleaseDate = book.ReleaseDate,
                    Overview = book.Overview,
                    PageCount = book.PageCount,
                    Isbn = book.Metadata.Value?.Isbn13,
                    Asin = book.Metadata.Value?.Asin,
                    AuthorName = book.Author?.Value?.Name ?? book.Metadata.Value?.AuthorName,
                    Images = book.Metadata.Value?.Images ?? new List<MediaCover>(),
                    Ratings = book.Ratings,
                    Genres = book.Genres
                };

                var existingBook = _bookService.FindByForeignId(book.ForeignBookId);
                if (existingBook != null)
                {
                    resource.IsExisting = true;
                    resource.Id = existingBook.Id;
                }

                _coverMapper.ConvertToLocalUrls(0, MediaCoverEntity.Book, resource.Images);
                
                resources.Add(resource);
            }

            return resources;
        }

        private bool IsIsbn(string term)
        {
            var cleanTerm = term.Replace("-", "").Replace(" ", "");
            return (cleanTerm.Length == 10 || cleanTerm.Length == 13) && 
                   cleanTerm.All(char.IsDigit);
        }

        private bool IsAsin(string term)
        {
            return term.Length == 10 && 
                   term.StartsWith("B") && 
                   term.Skip(1).All(c => char.IsLetterOrDigit(c));
        }

        private bool IsGoodreadsId(string term)
        {
            return term.All(char.IsDigit) && term.Length >= 1 && term.Length <= 10;
        }
    }
}