using Dapper;
using NLog;
using Readarr.Core.Datastore;

namespace Readarr.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedMetadata : IHousekeepingTask
    {
        private readonly IMainDatabase _database;
        private readonly Logger _logger;

        public CleanupOrphanedMetadata(IMainDatabase database, Logger logger)
        {
            _database = database;
            _logger = logger;
        }

        public void Clean()
        {
            _logger.Debug("Running cleanup of orphaned metadata");

            using var mapper = _database.OpenConnection();

            // Delete author metadata not linked to any authors
            mapper.Execute(@"DELETE FROM ""AuthorMetadata""
                                     WHERE ""Id"" NOT IN (
                                         SELECT DISTINCT ""AuthorMetadataId"" FROM ""Authors""
                                     )");

            // Delete book metadata not linked to any books
            mapper.Execute(@"DELETE FROM ""BookMetadata""
                                     WHERE ""Id"" NOT IN (
                                         SELECT DISTINCT ""BookMetadataId"" FROM ""Books""
                                     )");

            // Delete editions not linked to any books
            mapper.Execute(@"DELETE FROM ""Editions""
                                     WHERE ""BookId"" NOT IN (
                                         SELECT DISTINCT ""Id"" FROM ""Books""
                                     )");

            // Delete series book links where the book doesn't exist
            mapper.Execute(@"DELETE FROM ""SeriesBookLink""
                                     WHERE ""BookId"" NOT IN (
                                         SELECT DISTINCT ""Id"" FROM ""Books""
                                     )");

            // Delete series book links where the series doesn't exist
            mapper.Execute(@"DELETE FROM ""SeriesBookLink""
                                     WHERE ""SeriesId"" NOT IN (
                                         SELECT DISTINCT ""Id"" FROM ""Series""
                                     )");
        }
    }
}