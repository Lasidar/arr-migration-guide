﻿using FluentMigrator;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Datastore.Migration
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
                  .WithColumn("SeriesId").AsInt32().NotNullable()
                  .WithColumn("Consumer").AsString().NotNullable()
                  .WithColumn("Type").AsInt32().NotNullable()
                  .WithColumn("RelativePath").AsString().NotNullable()
                  .WithColumn("LastUpdated").AsDateTime().NotNullable()
                  .WithColumn("SeasonNumber").AsInt32().Nullable()
                  .WithColumn("EpisodeFileId").AsInt32().Nullable();
        }
    }
}
