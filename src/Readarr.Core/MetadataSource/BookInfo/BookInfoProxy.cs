using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NLog;
using Readarr.Common.Cloud;
using Readarr.Common.Extensions;
using Readarr.Common.Http;
using Readarr.Core.Books;
using Readarr.Core.Exceptions;
using Readarr.Core.Http;
using Readarr.Core.MediaCover;
using Readarr.Core.MetadataSource.BookInfo.Resources;
using Readarr.Core.Parser;

namespace Readarr.Core.MetadataSource.BookInfo
{
    public class BookInfoProxy : IProvideAuthorInfo, IProvideBookInfo, ISearchForNewEntity
    {
        private readonly IHttpClient _httpClient;
        private readonly IHttpRequestBuilderFactory _requestBuilder;
        private readonly IAuthorService _authorService;
        private readonly IBookService _bookService;
        private readonly Logger _logger;

        public BookInfoProxy(IHttpClient httpClient,
                            IReadarrCloudRequestBuilder cloudRequestBuilder,
                            IAuthorService authorService,
                            IBookService bookService,
                            Logger logger)
        {
            _httpClient = httpClient;
            _requestBuilder = cloudRequestBuilder.BookInfo;
            _authorService = authorService;
            _bookService = bookService;
            _logger = logger;
        }

        public Tuple<Author, List<Book>> GetAuthorInfo(string foreignAuthorId)
        {
            _logger.Debug("Getting Author details GoodreadsId: {0}", foreignAuthorId);

            var httpRequest = _requestBuilder.Create()
                .SetSegment("route", "author")
                .SetSegment("id", foreignAuthorId)
                .Build();

            httpRequest.AllowAutoRedirect = true;
            httpRequest.SuppressHttpError = true;

            var httpResponse = _httpClient.Get<AuthorResource>(httpRequest);

            if (httpResponse.HasHttpError)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new AuthorNotFoundException(foreignAuthorId);
                }
                else
                {
                    throw new HttpException(httpRequest, httpResponse);
                }
            }

            var author = MapAuthor(httpResponse.Resource);
            var books = httpResponse.Resource.Books?.Select(MapBook).ToList() ?? new List<Book>();

            return new Tuple<Author, List<Book>>(author, books);
        }

        public List<Author> SearchForNewAuthor(string title)
        {
            var httpRequest = _requestBuilder.Create()
                .SetSegment("route", "search/author")
                .AddQueryParam("q", title.ToLower().Trim())
                .Build();

            var httpResponse = _httpClient.Get<List<AuthorResource>>(httpRequest);

            return httpResponse.Resource.SelectList(MapSearchResult);
        }

        public AuthorMetadata GetAuthorMetadata(string foreignAuthorId)
        {
            var httpRequest = _requestBuilder.Create()
                .SetSegment("route", "author")
                .SetSegment("id", foreignAuthorId)
                .Build();

            var httpResponse = _httpClient.Get<AuthorResource>(httpRequest);

            if (httpResponse.HasHttpError)
            {
                throw new AuthorNotFoundException(foreignAuthorId);
            }

            return MapAuthorMetadata(httpResponse.Resource);
        }

        public Book GetBookInfo(string foreignBookId)
        {
            _logger.Debug("Getting Book details GoodreadsId: {0}", foreignBookId);

            var httpRequest = _requestBuilder.Create()
                .SetSegment("route", "book")
                .SetSegment("id", foreignBookId)
                .Build();

            httpRequest.AllowAutoRedirect = true;
            httpRequest.SuppressHttpError = true;

            var httpResponse = _httpClient.Get<BookResource>(httpRequest);

            if (httpResponse.HasHttpError)
            {
                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new BookNotFoundException(foreignBookId);
                }
                else
                {
                    throw new HttpException(httpRequest, httpResponse);
                }
            }

            return MapBook(httpResponse.Resource);
        }

        public List<Book> GetBooksForAuthor(int authorMetadataId)
        {
            var author = _authorService.GetAuthorByMetadataId(authorMetadataId);
            
            if (author == null)
            {
                return new List<Book>();
            }

            var httpRequest = _requestBuilder.Create()
                .SetSegment("route", "author")
                .SetSegment("id", author.Metadata.Value.ForeignAuthorId)
                .AddQueryParam("includeBooks", "true")
                .Build();

            var httpResponse = _httpClient.Get<AuthorResource>(httpRequest);

            return httpResponse.Resource.Books?.Select(MapBook).ToList() ?? new List<Book>();
        }

        public List<Book> SearchForNewBook(string title)
        {
            var httpRequest = _requestBuilder.Create()
                .SetSegment("route", "search/book")
                .AddQueryParam("q", title.ToLower().Trim())
                .Build();

            var httpResponse = _httpClient.Get<List<BookResource>>(httpRequest);

            return httpResponse.Resource.SelectList(MapBook);
        }

        public List<Book> SearchForNewBookByIsbn(string isbn)
        {
            var httpRequest = _requestBuilder.Create()
                .SetSegment("route", "search/book")
                .AddQueryParam("isbn", isbn.Trim())
                .Build();

            var httpResponse = _httpClient.Get<List<BookResource>>(httpRequest);

            return httpResponse.Resource.SelectList(MapBook);
        }

        public List<object> SearchForNewEntity(string title)
        {
            var result = new List<object>();

            // Search for authors
            try
            {
                var authors = SearchForNewAuthor(title);
                result.AddRange(authors);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Error searching for authors");
            }

            // Search for books
            try
            {
                var books = SearchForNewBook(title);
                result.AddRange(books);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Error searching for books");
            }

            return result;
        }

        private Author MapAuthor(AuthorResource resource)
        {
            var author = new Author
            {
                Metadata = MapAuthorMetadata(resource),
                CleanName = resource.AuthorName.CleanAuthorName(),
                SortName = resource.AuthorNameLastFirst,
                Monitored = true,
                MonitorNewItems = MonitorTypes.All
            };

            if (resource.Books != null)
            {
                author.Books = resource.Books.Select(MapBook).ToList();
            }

            if (resource.Series != null)
            {
                author.Series = resource.Series.Select(MapSeries).ToList();
            }

            return author;
        }

        private AuthorMetadata MapAuthorMetadata(AuthorResource resource)
        {
            var metadata = new AuthorMetadata
            {
                ForeignAuthorId = resource.ForeignAuthorId,
                GoodreadsId = resource.GoodreadsId,
                IsniId = resource.IsniId,
                AsinId = resource.AsinId,
                Name = resource.AuthorName,
                Overview = resource.Overview,
                Gender = resource.Gender,
                Born = resource.Born,
                Died = resource.Died,
                Website = resource.Website,
                Status = MapAuthorStatus(resource.Status),
                SortName = resource.AuthorNameLastFirst,
                NameLastFirst = resource.AuthorNameLastFirst
            };

            if (resource.Images != null)
            {
                metadata.Images = resource.Images.Select(MapImage).ToList();
            }

            if (resource.Genres != null)
            {
                metadata.Genres = resource.Genres;
            }

            if (resource.Links != null)
            {
                metadata.Links = resource.Links.Select(MapLink).ToList();
            }

            if (resource.Aliases != null)
            {
                metadata.Aliases = resource.Aliases;
            }

            if (resource.Ratings != null)
            {
                metadata.Ratings = MapRatings(resource.Ratings);
            }

            return metadata;
        }

        private Author MapSearchResult(AuthorResource resource)
        {
            var author = MapAuthor(resource);
            
            // For search results, we might have limited data
            author.Monitored = false;
            
            return author;
        }

        private Book MapBook(BookResource resource)
        {
            var book = new Book
            {
                ForeignBookId = resource.ForeignBookId,
                Monitored = true,
                AnyEditionOk = true,
                Metadata = MapBookMetadata(resource),
                CleanTitle = resource.Title.CleanBookTitle(),
                SortTitle = resource.TitleSlug
            };

            if (resource.Editions != null)
            {
                book.Editions = resource.Editions.Select(MapEdition).ToList();
            }

            if (resource.SeriesLinks != null)
            {
                book.SeriesId = resource.SeriesLinks.FirstOrDefault()?.SeriesId;
                book.SeriesPosition = resource.SeriesLinks.FirstOrDefault()?.Position;
            }

            return book;
        }

        private BookMetadata MapBookMetadata(BookResource resource)
        {
            return new BookMetadata
            {
                ForeignBookId = resource.ForeignBookId,
                GoodreadsId = resource.GoodreadsId,
                Isbn = resource.Isbn,
                Isbn13 = resource.Isbn13,
                Asin = resource.Asin,
                Title = resource.Title,
                SortTitle = resource.TitleSlug,
                TitleSlug = resource.TitleSlug,
                OriginalTitle = resource.OriginalTitle,
                Language = resource.Language,
                Overview = resource.Overview,
                Publisher = resource.Publisher,
                ReleaseDate = resource.ReleaseDate,
                PageCount = resource.PageCount,
                Images = resource.Images?.Select(MapImage).ToList() ?? new List<MediaCover>(),
                Genres = resource.Genres ?? new List<string>(),
                Links = resource.Links?.Select(MapLink).ToList() ?? new List<Links>(),
                Ratings = resource.Ratings != null ? MapRatings(resource.Ratings) : new Ratings()
            };
        }

        private Edition MapEdition(EditionResource resource)
        {
            return new Edition
            {
                ForeignEditionId = resource.ForeignEditionId,
                Isbn = resource.Isbn,
                Isbn13 = resource.Isbn13,
                Asin = resource.Asin,
                Title = resource.Title,
                Language = resource.Language,
                Overview = resource.Overview,
                Format = resource.Format,
                IsEbook = resource.IsEbook,
                Publisher = resource.Publisher,
                PageCount = resource.PageCount,
                ReleaseDate = resource.ReleaseDate,
                Images = resource.Images?.Select(MapImage).ToList() ?? new List<MediaCover>(),
                Ratings = resource.Ratings != null ? MapRatings(resource.Ratings) : new Ratings(),
                Monitored = true
            };
        }

        private Series MapSeries(SeriesResource resource)
        {
            return new Series
            {
                ForeignSeriesId = resource.ForeignSeriesId,
                Title = resource.Title,
                Description = resource.Description,
                Numbered = resource.Numbered
            };
        }

        private MediaCover MapImage(ImageResource resource)
        {
            return new MediaCover
            {
                Url = resource.Url,
                CoverType = MapCoverType(resource.CoverType)
            };
        }

        private MediaCoverTypes MapCoverType(string coverType)
        {
            switch (coverType?.ToLower())
            {
                case "poster":
                    return MediaCoverTypes.Poster;
                case "banner":
                    return MediaCoverTypes.Banner;
                case "fanart":
                    return MediaCoverTypes.Fanart;
                case "cover":
                    return MediaCoverTypes.Cover;
                case "disc":
                    return MediaCoverTypes.Disc;
                default:
                    return MediaCoverTypes.Unknown;
            }
        }

        private Links MapLink(LinkResource resource)
        {
            return new Links
            {
                Url = resource.Url,
                Name = resource.Name
            };
        }

        private Ratings MapRatings(RatingsResource resource)
        {
            return new Ratings
            {
                Votes = resource.Count,
                Value = resource.Value,
                Popularity = resource.Popularity ?? 0
            };
        }

        private AuthorStatusType MapAuthorStatus(string status)
        {
            switch (status?.ToLower())
            {
                case "continuing":
                    return AuthorStatusType.Continuing;
                case "ended":
                case "dead":
                    return AuthorStatusType.Ended;
                default:
                    return AuthorStatusType.Continuing;
            }
        }
    }

    public interface ISearchForNewEntity
    {
        List<object> SearchForNewEntity(string title);
    }
}