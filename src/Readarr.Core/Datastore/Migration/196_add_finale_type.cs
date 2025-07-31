using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
{
    [Migration(196)]
    public class add_finale_type : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Alter.Table("Episodes").AddColumn("FinaleType").AsString().Nullable();
        }
    }
}
