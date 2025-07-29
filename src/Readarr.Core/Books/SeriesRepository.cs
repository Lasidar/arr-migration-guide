using System.Collections.Generic;
using System.Linq;
using Dapper;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public interface ISeriesRepository : IBasicRepository<Series>
    {
        Series FindByForeignSeriesId(string foreignSeriesId);
        Series FindByTitle(string title);
        List<Series> GetByAuthorId(int authorId);
        List<Series> FindByTitleInexact(string title);
        Dictionary<int, List<int>> GetAllSeriesBookIds();
        void InsertSeriesBookLink(SeriesBookLink link);
        void DeleteSeriesBookLink(int seriesId, int bookId);
        List<SeriesBookLink> GetSeriesBookLinks(int seriesId);
    }

    public class SeriesRepository : BasicRepository<Series>, ISeriesRepository
    {
        public SeriesRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public Series FindByForeignSeriesId(string foreignSeriesId)
        {
            return Query(s => s.ForeignSeriesId == foreignSeriesId).SingleOrDefault();
        }

        public Series FindByTitle(string title)
        {
            var cleanTitle = title.ToLowerInvariant();
            return Query(s => s.CleanTitle == cleanTitle).SingleOrDefault();
        }

        public List<Series> GetByAuthorId(int authorId)
        {
            return Query(s => s.AuthorId == authorId).ToList();
        }

        public List<Series> FindByTitleInexact(string title)
        {
            var builder = Builder().Where($"instr(@title, \"Series\".\"CleanTitle\")", new { title = title.ToLowerInvariant() });

            if (_database.DatabaseType == DatabaseType.PostgreSQL)
            {
                builder = Builder().Where($"(strpos(@title, \"Series\".\"CleanTitle\") > 0)", new { title = title.ToLowerInvariant() });
            }

            return Query(builder).ToList();
        }

        public Dictionary<int, List<int>> GetAllSeriesBookIds()
        {
            using (var conn = _database.OpenConnection())
            {
                var sql = "SELECT \"SeriesId\" AS Key, \"BookId\" AS Value FROM \"SeriesBookLink\"";
                var links = conn.Query<KeyValuePair<int, int>>(sql);
                
                return links.GroupBy(x => x.Key)
                           .ToDictionary(g => g.Key, g => g.Select(x => x.Value).ToList());
            }
        }

        public void InsertSeriesBookLink(SeriesBookLink link)
        {
            using (var conn = _database.OpenConnection())
            {
                var sql = @"INSERT INTO ""SeriesBookLink"" (""SeriesId"", ""BookId"", ""Position"", ""PositionOrder"") 
                           VALUES (@SeriesId, @BookId, @Position, @PositionOrder)";
                conn.Execute(sql, link);
            }
        }

        public void DeleteSeriesBookLink(int seriesId, int bookId)
        {
            using (var conn = _database.OpenConnection())
            {
                var sql = @"DELETE FROM ""SeriesBookLink"" WHERE ""SeriesId"" = @seriesId AND ""BookId"" = @bookId";
                conn.Execute(sql, new { seriesId, bookId });
            }
        }

        public List<SeriesBookLink> GetSeriesBookLinks(int seriesId)
        {
            using (var conn = _database.OpenConnection())
            {
                var sql = @"SELECT * FROM ""SeriesBookLink"" WHERE ""SeriesId"" = @seriesId ORDER BY ""PositionOrder""";
                return conn.Query<SeriesBookLink>(sql, new { seriesId }).ToList();
            }
        }
    }
}