using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(76)]
    public class add_users_table : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.TableForModel("Users")
                  .WithColumn("Identifier").AsString().NotNullable().Unique()
                  .WithColumn("Username").AsString().NotNullable().Unique()
                  .WithColumn("Password").AsString().NotNullable();
        }
    }
}
