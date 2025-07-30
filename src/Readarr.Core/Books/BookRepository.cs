using System.Collections.Generic;
using System.Linq;
using Dapper;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public interface IBookRepository : IBasicRepository<Book>
    {
        List<Book> GetBooks(int authorId);
        List<Book> GetBooksByAuthorMetadataId(int authorMetadataId);
        Book FindByForeignBookId(string foreignBookId);
        Book FindByIsbn(string isbn);
        Book FindByTitle(int authorMetadataId, string title);
        List<Book> FindByTitle(string title);
        List<Book> GetBooksWithoutMetadata();
        List<Book> GetBooksBySeriesId(int seriesId);
        void SetMonitoredFlag(int bookId, bool monitored);
        void SetMonitoredFlagForBooks(List<int> bookIds, bool monitored);
        List<Book> BooksBetweenDates(System.DateTime startDate, System.DateTime endDate, bool includeUnmonitored);
        List<Book> AuthorBooksBetweenDates(Author author, System.DateTime startDate, System.DateTime endDate, bool includeUnmonitored);
        void SetBookMonitored(int bookId, bool monitored);
        void SetBooksMonitored(List<int> bookIds, bool monitored);
        PagingSpec<Book> BooksWithoutFiles(PagingSpec<Book> pagingSpec);
    }

    public class BookRepository : BasicRepository<Book>, IBookRepository
    {
        public BookRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public List<Book> GetBooks(int authorId)
        {
            return Query(s => s.AuthorId == authorId).ToList();
        }

        public List<Book> GetBooksByAuthorMetadataId(int authorMetadataId)
        {
            var sql = @"SELECT Books.* FROM Books 
                        JOIN Authors ON Books.AuthorId = Authors.Id 
                        WHERE Authors.AuthorMetadataId = @authorMetadataId";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Book>(sql, new { authorMetadataId }).ToList();
            }
        }

        public Book FindByForeignBookId(string foreignBookId)
        {
            var sql = @"SELECT Books.* FROM Books 
                        JOIN BookMetadata ON Books.BookMetadataId = BookMetadata.Id 
                        WHERE BookMetadata.ForeignBookId = @foreignBookId";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Book>(sql, new { foreignBookId }).SingleOrDefault();
            }
        }

        public Book FindByIsbn(string isbn)
        {
            var sql = @"SELECT Books.* FROM Books 
                        JOIN BookMetadata ON Books.BookMetadataId = BookMetadata.Id 
                        WHERE BookMetadata.Isbn = @isbn OR BookMetadata.Isbn13 = @isbn";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Book>(sql, new { isbn }).SingleOrDefault();
            }
        }

        public Book FindByTitle(int authorMetadataId, string title)
        {
            var cleanTitle = title.ToLowerInvariant();
            var sql = @"SELECT Books.* FROM Books 
                        JOIN Authors ON Books.AuthorId = Authors.Id 
                        WHERE Authors.AuthorMetadataId = @authorMetadataId 
                        AND Books.CleanTitle = @cleanTitle";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Book>(sql, new { authorMetadataId, cleanTitle }).SingleOrDefault();
            }
        }

        public List<Book> FindByTitle(string title)
        {
            var cleanTitle = title.ToLowerInvariant();
            return Query(b => b.CleanTitle == cleanTitle).ToList();
        }

        public List<Book> GetBooksWithoutMetadata()
        {
            return Query(b => b.BookMetadataId == 0).ToList();
        }

        public List<Book> GetBooksBySeriesId(int seriesId)
        {
            return Query(b => b.SeriesId == seriesId).ToList();
        }

        public void SetMonitoredFlag(int bookId, bool monitored)
        {
            SetFields(new Book { Id = bookId, Monitored = monitored }, book => book.Monitored);
        }

        public void SetMonitoredFlagForBooks(List<int> bookIds, bool monitored)
        {
            var books = bookIds.Select(x => new Book { Id = x, Monitored = monitored }).ToList();
            SetFields(books, book => book.Monitored);
        }

        public List<Book> BooksBetweenDates(System.DateTime startDate, System.DateTime endDate, bool includeUnmonitored)
        {
            var sql = @"SELECT Books.* FROM Books 
                        JOIN BookMetadata ON Books.BookMetadataId = BookMetadata.Id 
                        WHERE BookMetadata.ReleaseDate >= @startDate 
                        AND BookMetadata.ReleaseDate <= @endDate";

            if (!includeUnmonitored)
            {
                sql += " AND Books.Monitored = 1";
            }

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Book>(sql, new { startDate, endDate }).ToList();
            }
        }

        public List<Book> AuthorBooksBetweenDates(Author author, System.DateTime startDate, System.DateTime endDate, bool includeUnmonitored)
        {
            var sql = @"SELECT Books.* FROM Books 
                        JOIN BookMetadata ON Books.BookMetadataId = BookMetadata.Id 
                        WHERE Books.AuthorId = @authorId 
                        AND BookMetadata.ReleaseDate >= @startDate 
                        AND BookMetadata.ReleaseDate <= @endDate";

            if (!includeUnmonitored)
            {
                sql += " AND Books.Monitored = 1";
            }

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Book>(sql, new { authorId = author.Id, startDate, endDate }).ToList();
            }
        }

        public void SetBookMonitored(int bookId, bool monitored)
        {
            SetMonitoredFlag(bookId, monitored);
        }

        public void SetBooksMonitored(List<int> bookIds, bool monitored)
        {
            SetMonitoredFlagForBooks(bookIds, monitored);
        }

        public PagingSpec<Book> BooksWithoutFiles(PagingSpec<Book> pagingSpec)
        {
            // This would need to be implemented with proper paging logic
            // For now, return a simple implementation
            // TODO: Implement proper query that checks editions without files
            var allBooksWithoutFiles = All().Where(b => !b.Editions.IsLoaded || b.Editions.Value.All(e => e.BookFileId == 0)).ToList();
            
            pagingSpec.Records = allBooksWithoutFiles
                .Skip((pagingSpec.Page - 1) * pagingSpec.PageSize)
                .Take(pagingSpec.PageSize)
                .ToList();
            
            pagingSpec.TotalRecords = allBooksWithoutFiles.Count();
            
            return pagingSpec;
        }
    }
}