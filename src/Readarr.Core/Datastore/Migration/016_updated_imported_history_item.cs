using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(16)]
    public class updated_imported_history_item : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Execute.Sql("UPDATE \"History\" SET \"Data\" = replace( \"Data\", '\"Path\"', '\"ImportedPath\"' ) WHERE \"EventType\" = 3");
        }
    }
}
