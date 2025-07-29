using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(5)]
    public class added_eventtype_to_history : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("History")
                .AddColumn("EventType")
                .AsInt32()
                .Nullable();
        }
    }
}
