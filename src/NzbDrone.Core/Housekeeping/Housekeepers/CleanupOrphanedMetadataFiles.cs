using Dapper;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedMetadataFiles : IHousekeepingTask
    {
        private readonly IMainDatabase _database;

        public CleanupOrphanedMetadataFiles(IMainDatabase database)
        {
            _database = database;
        }

        public void Clean()
        {
            DeleteOrphanedBySeries();
            DeleteOrphanedByEditionFile();
            DeleteWhereEditionFileIsZero();
        }

        private void DeleteOrphanedBySeries()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""MetadataFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""MetadataFiles"".""Id"" FROM ""MetadataFiles""
                                     LEFT OUTER JOIN ""Series""
                                     ON ""MetadataFiles"".""AuthorId"" = ""Series"".""Id""
                                     WHERE ""Series"".""Id"" IS NULL)");
        }

        private void DeleteOrphanedByEditionFile()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""MetadataFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""MetadataFiles"".""Id"" FROM ""MetadataFiles""
                                     LEFT OUTER JOIN ""EditionFiles""
                                     ON ""MetadataFiles"".""EditionFileId"" = ""EditionFiles"".""Id""
                                     WHERE ""MetadataFiles"".""EditionFileId"" > 0
                                     AND ""EditionFiles"".""Id"" IS NULL)");
        }

        private void DeleteWhereEditionFileIsZero()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""MetadataFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""Id"" FROM ""MetadataFiles""
                                     WHERE ""Type"" IN (2, 5)
                                     AND ""EditionFileId"" = 0)");
        }
    }
}
