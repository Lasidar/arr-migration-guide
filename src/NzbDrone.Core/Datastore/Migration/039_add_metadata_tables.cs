using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(39)]
    public class add_metadata_tables : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.TableForModel("Metadata")
                  .WithColumn("Enable").AsBoolean().NotNullable()
                  .WithColumn("Name").AsString().NotNullable()
                  .WithColumn("Implementation").AsString().NotNullable()
                  .WithColumn("Settings").AsString().NotNullable()
                  .WithColumn("ConfigContract").AsString().NotNullable();

            Create.TableForModel("MetadataFiles")
                  .WithColumn("AuthorId").AsInt32().NotNullable()
                  .WithColumn("Consumer").AsString().NotNullable()
                  .WithColumn("Type").AsInt32().NotNullable()
                  .WithColumn("RelativePath").AsString().NotNullable()
                  .WithColumn("LastUpdated").AsDateTime().NotNullable()
                  .WithColumn("BookNumber").AsInt32().Nullable()
                  .WithColumn("EditionFileId").AsInt32().Nullable();
        }
    }
}
