using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Qualities;
using Readarr.Core.Books;

namespace Readarr.Core.History
{
    public interface IHistoryRepository : IBasicRepository<BookHistory>
    {
        BookHistory MostRecentForBook(int bookId);
        List<BookHistory> FindByBookId(int bookId);
        BookHistory MostRecentForDownloadId(string downloadId);
        List<BookHistory> FindByDownloadId(string downloadId);
        List<BookHistory> GetByAuthor(int authorId, HistoryEventType? eventType);
        List<BookHistory> GetByBook(int bookId, HistoryEventType? eventType);
        List<BookHistory> FindDownloadHistory(int authorId, QualityModel quality);
        void DeleteForAuthor(List<int> authorIds);
        List<BookHistory> Since(DateTime date, HistoryEventType? eventType);
        PagingSpec<BookHistory> GetPaged(PagingSpec<BookHistory> pagingSpec, int[] languages, int[] qualities);
    }

    public class HistoryRepository : BasicRepository<BookHistory>, IHistoryRepository
    {
        public HistoryRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public BookHistory MostRecentForBook(int bookId)
        {
            return Query(h => h.BookId == bookId).MaxBy(h => h.Date);
        }

        public List<BookHistory> FindByBookId(int bookId)
        {
            return Query(h => h.BookId == bookId)
                        .OrderByDescending(h => h.Date)
                        .ToList();
        }

        public BookHistory MostRecentForDownloadId(string downloadId)
        {
            return Query(h => h.DownloadId == downloadId).MaxBy(h => h.Date);
        }

        public List<BookHistory> FindByDownloadId(string downloadId)
        {
            return Query(h => h.DownloadId == downloadId);
        }

        public List<BookHistory> GetByAuthor(int authorId, HistoryEventType? eventType)
        {
            var builder = Builder().Join<BookHistory, Author>((h, a) => h.AuthorId == a.Id)
                                   .Join<BookHistory, Book>((h, b) => h.BookId == b.Id)
                                   .Where<BookHistory>(h => h.AuthorId == authorId);

            if (eventType.HasValue)
            {
                builder.Where<BookHistory>(h => h.EventType == eventType);
            }

            return Query(builder).OrderByDescending(h => h.Date).ToList();
        }

        public List<BookHistory> GetByBook(int bookId, HistoryEventType? eventType)
        {
            var builder = Builder()
                .Where<BookHistory>(h => h.BookId == bookId);

            if (eventType.HasValue)
            {
                builder.Where<BookHistory>(h => h.EventType == eventType);
            }

            return Query(builder).OrderByDescending(h => h.Date).ToList();
        }

        public List<BookHistory> FindDownloadHistory(int authorId, QualityModel quality)
        {
            return Query(h =>
                 h.AuthorId == authorId &&
                 h.Quality == quality &&
                 (h.EventType == HistoryEventType.Grabbed ||
                 h.EventType == HistoryEventType.DownloadFailed ||
                 h.EventType == HistoryEventType.DownloadFolderImported))
                 .ToList();
        }

        public void DeleteForAuthor(List<int> authorIds)
        {
            Delete(c => authorIds.Contains(c.AuthorId));
        }

        public List<BookHistory> Since(DateTime date, HistoryEventType? eventType)
        {
            var builder = Builder()
                .Join<BookHistory, Author>((h, a) => h.AuthorId == a.Id)
                .Join<BookHistory, Book>((h, b) => h.BookId == b.Id)
                .Where<BookHistory>(x => x.Date >= date);

            if (eventType.HasValue)
            {
                builder.Where<BookHistory>(h => h.EventType == eventType);
            }

            return _database.QueryJoined<BookHistory, Author, Book>(builder, (history, author, book) =>
            {
                history.Author = author;
                history.Book = book;
                return history;
            }).OrderBy(h => h.Date).ToList();
        }

        public PagingSpec<BookHistory> GetPaged(PagingSpec<BookHistory> pagingSpec, int[] languages, int[] qualities)
        {
            pagingSpec.Records = GetPagedRecords(PagedBuilder(languages, qualities), pagingSpec, PagedQuery);

            var countTemplate = $"SELECT COUNT(*) FROM (SELECT /**select**/ FROM \"{TableMapping.Mapper.TableNameMapping(typeof(BookHistory))}\" /**join**/ /**innerjoin**/ /**leftjoin**/ /**where**/ /**groupby**/ /**having**/) AS \"Inner\"";
            pagingSpec.TotalRecords = GetPagedRecordCount(PagedBuilder(languages, qualities).Select(typeof(BookHistory)), pagingSpec, countTemplate);

            return pagingSpec;
        }

        private SqlBuilder PagedBuilder(int[] languages, int[] qualities)
        {
            var builder = Builder()
                .Join<BookHistory, Author>((h, a) => h.AuthorId == a.Id)
                .Join<BookHistory, Book>((h, b) => h.BookId == b.Id);

            if (languages is { Length: > 0 })
            {
                builder.Where($"({BuildLanguageWhereClause(languages)})");
            }

            if (qualities is { Length: > 0 })
            {
                builder.Where($"({BuildQualityWhereClause(qualities)})");
            }

            return builder;
        }

        protected override IEnumerable<BookHistory> PagedQuery(SqlBuilder builder) =>
            _database.QueryJoined<BookHistory, Author, Book>(builder, (history, author, book) =>
            {
                history.Author = author;
                history.Book = book;
                return history;
            });

        private string BuildLanguageWhereClause(int[] languages)
        {
            var clauses = new List<string>();

            foreach (var language in languages)
            {
                // There are 4 different types of values we should see:
                // - Not the last value in the array
                // - When it's the last value in the array and on different OSes
                // - When it was converted from a single language

                clauses.Add($"\"{TableMapping.Mapper.TableNameMapping(typeof(BookHistory))}\".\"Languages\" LIKE '[% {language},%]'");
                clauses.Add($"\"{TableMapping.Mapper.TableNameMapping(typeof(BookHistory))}\".\"Languages\" LIKE '[% {language}' || CHAR(13) || '%]'");
                clauses.Add($"\"{TableMapping.Mapper.TableNameMapping(typeof(BookHistory))}\".\"Languages\" LIKE '[% {language}' || CHAR(10) || '%]'");
                clauses.Add($"\"{TableMapping.Mapper.TableNameMapping(typeof(BookHistory))}\".\"Languages\" LIKE '[{language}]'");
            }

            return $"({string.Join(" OR ", clauses)})";
        }

        private string BuildQualityWhereClause(int[] qualities)
        {
            var clauses = new List<string>();

            foreach (var quality in qualities)
            {
                clauses.Add($"\"{TableMapping.Mapper.TableNameMapping(typeof(BookHistory))}\".\"Quality\" LIKE '%_quality_: {quality},%'");
            }

            return $"({string.Join(" OR ", clauses)})";
        }
    }
}
