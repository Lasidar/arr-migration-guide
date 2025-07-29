using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(27)]
    public class fix_omgwtfnzbs : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Update.Table("Indexers")
                  .Set(new { ConfigContract = "OmgwtfnzbsSettings" })
                  .Where(new { Implementation = "Omgwtfnzbs" });

            Update.Table("Indexers")
                  .Set(new { Settings = "{}" })
                  .Where(new { Implementation = "Omgwtfnzbs", Settings = (string)null });

            Update.Table("Indexers")
                  .Set(new { Settings = "{}" })
                  .Where(new { Implementation = "Omgwtfnzbs", Settings = "" });
        }
    }
}
