using Dapper;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Housekeeping.Housekeepers
{
    public class CleanupOrphanedEditionFiles : IHousekeepingTask
    {
        private readonly IMainDatabase _database;

        public CleanupOrphanedEditionFiles(IMainDatabase database)
        {
            _database = database;
        }

        public void Clean()
        {
            using var mapper = _database.OpenConnection();
            mapper.Execute(@"DELETE FROM ""EditionFiles""
                                     WHERE ""Id"" IN (
                                     SELECT ""EditionFiles"".""Id"" FROM ""EditionFiles""
                                     LEFT OUTER JOIN ""Episodes""
                                     ON ""EditionFiles"".""Id"" = ""Episodes"".""EditionFileId""
                                     WHERE ""Episodes"".""Id"" IS NULL)");
        }
    }
}
