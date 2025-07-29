using Dapper;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class CleanupDuplicateMetadataFiles : IHousekeepingTask
    {
        private readonly IMainDatabase _database;

        public CleanupDuplicateMetadataFiles(IMainDatabase database)
        {
            _database = database;
        }

        public void Clean()
        {
            DeleteDuplicateSeriesMetadata();
            DeleteDuplicateEpisodeMetadata();
            DeleteDuplicateEpisodeImages();
        }

        private void DeleteDuplicateSeriesMetadata()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""MetadataFiles""
                                     WHERE ""Id"" IN (
                                         SELECT MIN(""Id"") FROM ""MetadataFiles""
                                         WHERE ""Type"" = 1
                                         GROUP BY ""AuthorId"", ""Consumer""
                                         HAVING COUNT(""AuthorId"") > 1
                                     )");
        }

        private void DeleteDuplicateEpisodeMetadata()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""MetadataFiles""
                                     WHERE ""Id"" IN (
                                         SELECT MIN(""Id"") FROM ""MetadataFiles""
                                         WHERE ""Type"" = 2
                                         GROUP BY ""EditionFileId"", ""Consumer""
                                         HAVING COUNT(""EditionFileId"") > 1
                                     )");
        }

        private void DeleteDuplicateEpisodeImages()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""MetadataFiles""
                                     WHERE ""Id"" IN (
                                         SELECT MIN(""Id"") FROM ""MetadataFiles""
                                         WHERE ""Type"" = 5
                                         GROUP BY ""EditionFileId"", ""Consumer""
                                         HAVING COUNT(""EditionFileId"") > 1
                                     )");
        }
    }
}
