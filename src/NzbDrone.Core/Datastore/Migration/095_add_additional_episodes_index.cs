using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(95)]
    public class add_additional_episodes_index : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.Index().OnTable("Episodes").OnColumn("AuthorId").Ascending()
                                              .OnColumn("BookNumber").Ascending()
                                              .OnColumn("EditionNumber").Ascending();
        }
    }
}
