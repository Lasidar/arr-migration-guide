using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(63)]
    public class add_remotepathmappings : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.TableForModel("RemotePathMappings")
                  .WithColumn("Host").AsString()
                  .WithColumn("RemotePath").AsString()
                  .WithColumn("LocalPath").AsString();
        }
    }
}
