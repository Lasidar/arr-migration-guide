using Dapper;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedSubtitleFiles : IHousekeepingTask
    {
        private readonly IMainDatabase _database;

        public CleanupOrphanedSubtitleFiles(IMainDatabase database)
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
            mapper.Execute(@"DELETE FROM ""SubtitleFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""SubtitleFiles"".""Id"" FROM ""SubtitleFiles""
                                     LEFT OUTER JOIN ""Series""
                                     ON ""SubtitleFiles"".""AuthorId"" = ""Series"".""Id""
                                     WHERE ""Series"".""Id"" IS NULL)");
        }

        private void DeleteOrphanedByEditionFile()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""SubtitleFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""SubtitleFiles"".""Id"" FROM ""SubtitleFiles""
                                     LEFT OUTER JOIN ""EditionFiles""
                                     ON ""SubtitleFiles"".""EditionFileId"" = ""EditionFiles"".""Id""
                                     WHERE ""SubtitleFiles"".""EditionFileId"" > 0
                                     AND ""EditionFiles"".""Id"" IS NULL)");
        }

        private void DeleteWhereEditionFileIsZero()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""SubtitleFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""Id"" FROM ""SubtitleFiles""
                                     WHERE ""EditionFileId"" = 0)");
        }
    }
}
