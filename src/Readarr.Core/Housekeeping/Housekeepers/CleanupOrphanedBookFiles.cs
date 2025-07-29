using Dapper;
using NLog;
using Readarr.Core.Datastore;

namespace Readarr.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedBookFiles : IHousekeepingTask
    {
        private readonly IMainDatabase _database;
        private readonly Logger _logger;

        public CleanupOrphanedBookFiles(IMainDatabase database, Logger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Clean()
        {
            _logger.Debug("Running cleanup of orphaned book files");

            using var mapper = _database.OpenConnection();

            // Delete book files that don't have a matching book
            mapper.Execute(@"DELETE FROM ""BookFiles""
                                     WHERE ""Id"" NOT IN (
                                         SELECT DISTINCT ""BookFiles"".""Id"" FROM ""BookFiles""
                                         JOIN ""Books"" ON ""BookFiles"".""BookId"" = ""Books"".""Id""
                                     )");

            // Delete book files that don't have a matching edition
            mapper.Execute(@"DELETE FROM ""BookFiles""
                                     WHERE ""Id"" NOT IN (
                                         SELECT DISTINCT ""BookFiles"".""Id"" FROM ""BookFiles""
                                         JOIN ""Editions"" ON ""BookFiles"".""EditionId"" = ""Editions"".""Id""
                                     )");

            // Delete book files that don't have a matching author
            mapper.Execute(@"DELETE FROM ""BookFiles""
                                     WHERE ""Id"" NOT IN (
                                         SELECT DISTINCT ""BookFiles"".""Id"" FROM ""BookFiles""
                                         JOIN ""Authors"" ON ""BookFiles"".""AuthorId"" = ""Authors"".""Id""
                                     )");
        }
    }
}