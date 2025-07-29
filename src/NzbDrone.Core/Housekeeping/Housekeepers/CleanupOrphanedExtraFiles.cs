using Dapper;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedExtraFiles : IHousekeepingTask
    {
        private readonly IMainDatabase _database;

        public CleanupOrphanedExtraFiles(IMainDatabase database)
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
            mapper.Execute(@"DELETE FROM ""ExtraFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""ExtraFiles"".""Id"" FROM ""ExtraFiles""
                                     LEFT OUTER JOIN ""Series""
                                     ON ""ExtraFiles"".""AuthorId"" = ""Series"".""Id""
                                     WHERE ""Series"".""Id"" IS NULL)");
        }

        private void DeleteOrphanedByEditionFile()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""ExtraFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""ExtraFiles"".""Id"" FROM ""ExtraFiles""
                                     LEFT OUTER JOIN ""EditionFiles""
                                     ON ""ExtraFiles"".""EditionFileId"" = ""EditionFiles"".""Id""
                                     WHERE ""ExtraFiles"".""EditionFileId"" > 0
                                     AND ""EditionFiles"".""Id"" IS NULL)");
        }

        private void DeleteWhereEditionFileIsZero()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""ExtraFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""Id"" FROM ""ExtraFiles""
                                     WHERE ""EditionFileId"" = 0)");
        }
    }
}
