using System.Collections.Generic;
using System.Linq;
using Dapper;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public interface IAuthorRepository : IBasicRepository<Author>
    {
        bool AuthorPathExists(string path);
        Author FindByName(string cleanName);
        List<Author> FindByNameInexact(string cleanName);
        Author FindByForeignAuthorId(string foreignAuthorId);
        Author FindByGoodreadsId(string goodreadsId);
        Author FindByPath(string path);
        List<string> AllAuthorForeignIds();
        Dictionary<int, string> AllAuthorPaths();
        Dictionary<int, List<int>> AllAuthorTags();
        Author FindById(int authorId);
        List<Author> GetAuthorsWithoutMetadata();
    }

    public class AuthorRepository : BasicRepository<Author>, IAuthorRepository
    {
        public AuthorRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public bool AuthorPathExists(string path)
        {
            return Query(c => c.Path == path).Any();
        }

        public Author FindByName(string cleanName)
        {
            cleanName = cleanName.ToLowerInvariant();

            var authors = Query(s => s.CleanName == cleanName)
                                        .ToList();

            return ReturnSingleAuthorOrThrow(authors);
        }

        public List<Author> FindByNameInexact(string cleanName)
        {
            var builder = Builder().Where($"instr(@cleanName, \"Authors\".\"CleanName\")", new { cleanName = cleanName });

            if (_database.DatabaseType == DatabaseType.PostgreSQL)
            {
                builder = Builder().Where($"(strpos(@cleanName, \"Authors\".\"CleanName\") > 0)", new { cleanName = cleanName });
            }

            return Query(builder).ToList();
        }

        public Author FindByForeignAuthorId(string foreignAuthorId)
        {
            var sql = @"SELECT Authors.* FROM Authors 
                        JOIN AuthorMetadata ON Authors.AuthorMetadataId = AuthorMetadata.Id 
                        WHERE AuthorMetadata.ForeignAuthorId = @foreignAuthorId";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Author>(sql, new { foreignAuthorId }).SingleOrDefault();
            }
        }

        public Author FindByGoodreadsId(string goodreadsId)
        {
            var sql = @"SELECT Authors.* FROM Authors 
                        JOIN AuthorMetadata ON Authors.AuthorMetadataId = AuthorMetadata.Id 
                        WHERE AuthorMetadata.GoodreadsId = @goodreadsId";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<Author>(sql, new { goodreadsId }).SingleOrDefault();
            }
        }

        public Author FindByPath(string path)
        {
            return Query(s => s.Path == path)
                        .FirstOrDefault();
        }

        public List<string> AllAuthorForeignIds()
        {
            var sql = @"SELECT AuthorMetadata.ForeignAuthorId FROM Authors 
                        JOIN AuthorMetadata ON Authors.AuthorMetadataId = AuthorMetadata.Id";

            using (var conn = _database.OpenConnection())
            {
                return conn.Query<string>(sql).ToList();
            }
        }

        public Dictionary<int, string> AllAuthorPaths()
        {
            using (var conn = _database.OpenConnection())
            {
                var strSql = "SELECT \"Id\" AS Key, \"Path\" AS Value FROM \"Authors\"";
                return conn.Query<KeyValuePair<int, string>>(strSql).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Dictionary<int, List<int>> AllAuthorTags()
        {
            using (var conn = _database.OpenConnection())
            {
                var strSql = "SELECT \"Id\" AS Key, \"Tags\" AS Value FROM \"Authors\" WHERE \"Tags\" IS NOT NULL";
                return conn.Query<KeyValuePair<int, List<int>>>(strSql).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Author FindById(int authorId)
        {
            return Query(a => a.Id == authorId).SingleOrDefault();
        }

        public List<Author> GetAuthorsWithoutMetadata()
        {
            return Query(a => a.AuthorMetadataId == 0).ToList();
        }

        private Author ReturnSingleAuthorOrThrow(List<Author> authors)
        {
            if (authors.Count == 0)
            {
                return null;
            }

            if (authors.Count == 1)
            {
                return authors.First();
            }

            throw new MultipleAuthorsFoundException(authors, "Expected one author, but found {0}. Matching authors: {1}", authors.Count, string.Join(", ", authors));
        }
    }
}