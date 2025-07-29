using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(64)]
    public class remove_method_from_logs : NzbDroneMigrationBase
    {
        protected override void LogDbUpgrade()
        {
            Delete.Column("Method").FromTable("Logs");
        }
    }
}
