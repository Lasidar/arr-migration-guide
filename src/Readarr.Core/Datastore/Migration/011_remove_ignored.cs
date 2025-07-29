using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(11)]
    public class remove_ignored : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Delete.Column("Ignored").FromTable("Seasons");
            Delete.Column("Ignored").FromTable("Episodes");
        }
    }
}
