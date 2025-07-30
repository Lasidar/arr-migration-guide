using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using NLog;
using Readarr.Core.Datastore;
using Readarr.Core.MediaFiles;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Qualities;

namespace Readarr.Core.Books
{
    public interface IEditionRepository : IBasicRepository<Edition>
    {
        Edition FindByForeignEditionId(string foreignEditionId);
        Edition FindByIsbn(string isbn);
        List<Edition> GetEditions(int bookId);
        List<Edition> GetEditionsByAuthor(int authorId);
        List<Edition> GetEditionsByBookIds(List<int> bookIds);
        Edition GetEditionByFileId(int fileId);
        List<Edition> EditionsWithFiles(int bookId);
        PagingSpec<Edition> EditionsWithoutFiles(PagingSpec<Edition> pagingSpec);
        List<Edition> EditionsWithoutFiles(int authorId);
        PagingSpec<Edition> EditionsWhereCutoffUnmet(PagingSpec<Edition> pagingSpec, List<QualitiesBelowCutoff> qualitiesBelowCutoff);
        List<Edition> EditionsBetweenDates(DateTime startDate, DateTime endDate, bool includeUnmonitored);
        void SetMonitored(Edition edition, bool monitored);
        void SetMonitored(IEnumerable<int> ids, bool monitored);
        void SetFileId(Edition edition, int fileId);
        void ClearFileId(Edition edition, bool unmonitor);
        Edition FindByBookIdAndIsbn(int bookId, string isbn);
        List<Edition> GetAllMonitoredEditions();
    }

    public class EditionRepository : BasicRepository<Edition>, IEditionRepository
    {
        private readonly Logger _logger;

        public EditionRepository(IMainDatabase database, IEventAggregator eventAggregator, Logger logger)
            : base(database, eventAggregator)
        {
            _logger = logger;
        }

        protected override IEnumerable<Edition> PagedQuery(SqlBuilder builder) =>
            _database.QueryJoined<Edition, Book>(builder, (edition, book) =>
            {
                edition.Book = book;
                return edition;
            });

        public Edition FindByForeignEditionId(string foreignEditionId)
        {
            return Query(e => e.ForeignEditionId == foreignEditionId).SingleOrDefault();
        }

        public Edition FindByIsbn(string isbn)
        {
            return Query(e => e.Isbn == isbn || e.Isbn13 == isbn).SingleOrDefault();
        }

        public List<Edition> GetEditions(int bookId)
        {
            return Query(e => e.BookId == bookId).ToList();
        }

        public List<Edition> GetEditionsByAuthor(int authorId)
        {
            var sql = @"SELECT Editions.* FROM Editions 
                        JOIN Books ON Editions.BookId = Books.Id 
                        WHERE Books.AuthorId = @authorId";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Edition>(sql, new { authorId }).ToList();
            }
        }

        public List<Edition> GetEditionsByBookIds(List<int> bookIds)
        {
            return Query(e => bookIds.Contains(e.BookId)).ToList();
        }

        public Edition GetEditionByFileId(int fileId)
        {
            return Query(e => e.BookFileId == fileId).SingleOrDefault();
        }

        public List<Edition> EditionsWithFiles(int bookId)
        {
            return Query(e => e.BookId == bookId && e.BookFileId > 0).ToList();
        }

        public PagingSpec<Edition> EditionsWithoutFiles(PagingSpec<Edition> pagingSpec)
        {
            pagingSpec.Records = GetPagedRecords(Builder().Where<Edition>(e => e.BookFileId == 0), pagingSpec, PagedQuery);
            pagingSpec.TotalRecords = GetPagedRecordCount(Builder().Where<Edition>(e => e.BookFileId == 0), pagingSpec);

            return pagingSpec;
        }

        public List<Edition> EditionsWithoutFiles(int authorId)
        {
            var sql = @"SELECT Editions.* FROM Editions 
                        JOIN Books ON Editions.BookId = Books.Id 
                        WHERE Books.AuthorId = @authorId AND Editions.BookFileId = 0";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Edition>(sql, new { authorId }).ToList();
            }
        }

        public PagingSpec<Edition> EditionsWhereCutoffUnmet(PagingSpec<Edition> pagingSpec, List<QualitiesBelowCutoff> qualitiesBelowCutoff)
        {
            pagingSpec.Records = GetPagedRecords(BuildQualityCutoffQuery(qualitiesBelowCutoff), pagingSpec, PagedQuery);
            pagingSpec.TotalRecords = GetPagedRecordCount(BuildQualityCutoffQuery(qualitiesBelowCutoff), pagingSpec);

            return pagingSpec;
        }

        public List<Edition> EditionsBetweenDates(DateTime startDate, DateTime endDate, bool includeUnmonitored)
        {
            var builder = Builder()
                .Where<Edition>(e => e.ReleaseDate >= startDate && e.ReleaseDate <= endDate);

            if (!includeUnmonitored)
            {
                builder = builder.Where<Edition>(e => e.Monitored == true);
            }

            return Query(builder).ToList();
        }

        public void SetMonitored(Edition edition, bool monitored)
        {
            SetFields(edition, e => e.Monitored);
        }

        public void SetMonitored(IEnumerable<int> ids, bool monitored)
        {
            var editions = ids.Select(x => new Edition { Id = x, Monitored = monitored }).ToList();
            SetFields(editions, e => e.Monitored);
        }

        public void SetFileId(Edition edition, int fileId)
        {
            edition.BookFileId = fileId;
            SetFields(edition, e => e.BookFileId);
        }

        public void ClearFileId(Edition edition, bool unmonitor)
        {
            edition.BookFileId = 0;

            if (unmonitor)
            {
                edition.Monitored = false;
            }

            SetFields(edition, e => e.BookFileId, e => e.Monitored);
        }

        public Edition FindByBookIdAndIsbn(int bookId, string isbn)
        {
            return Query(e => e.BookId == bookId && (e.Isbn == isbn || e.Isbn13 == isbn)).SingleOrDefault();
        }

        public List<Edition> GetAllMonitoredEditions()
        {
            return Query(e => e.Monitored == true).ToList();
        }

        private SqlBuilder BuildQualityCutoffQuery(List<QualitiesBelowCutoff> qualitiesBelowCutoff)
        {
            var builder = Builder()
                .Where<Edition>(e => e.BookFileId > 0);

            if (qualitiesBelowCutoff.Any())
            {
                var qualityIds = qualitiesBelowCutoff.SelectMany(q => q.QualityIds).Distinct();
                var clauses = qualityIds.Select(id => $"(BookFiles.Quality = {id})");
                builder = builder.Where($"({string.Join(" OR ", clauses)})");
            }

            return builder;
        }
    }
}