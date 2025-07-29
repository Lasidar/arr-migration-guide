using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(99)]
    public class extra_and_subtitle_files : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.TableForModel("ExtraFiles")
                  .WithColumn("AuthorId").AsInt32().NotNullable()
                  .WithColumn("BookNumber").AsInt32().NotNullable()
                  .WithColumn("EditionFileId").AsInt32().NotNullable()
                  .WithColumn("RelativePath").AsString().NotNullable()
                  .WithColumn("Extension").AsString().NotNullable()
                  .WithColumn("Added").AsDateTime().NotNullable()
                  .WithColumn("LastUpdated").AsDateTime().NotNullable();

            Create.TableForModel("SubtitleFiles")
                  .WithColumn("AuthorId").AsInt32().NotNullable()
                  .WithColumn("BookNumber").AsInt32().NotNullable()
                  .WithColumn("EditionFileId").AsInt32().NotNullable()
                  .WithColumn("RelativePath").AsString().NotNullable()
                  .WithColumn("Extension").AsString().NotNullable()
                  .WithColumn("Added").AsDateTime().NotNullable()
                  .WithColumn("LastUpdated").AsDateTime().NotNullable()
                  .WithColumn("Language").AsInt32().NotNullable();

            Alter.Table("MetadataFiles")
                 .AddColumn("Added").AsDateTime().Nullable()
                 .AddColumn("Extension").AsString().Nullable();

            // Remove Metadata files that don't have an extension
            Execute.Sql("DELETE FROM \"MetadataFiles\" WHERE \"RelativePath\" NOT LIKE '%.%'");

            // Set Extension using the extension from RelativePath
            Execute.WithConnection(SetMetadataFileExtension);

            Alter.Table("MetadataFiles").AlterColumn("Extension").AsString().NotNullable();
        }

        private void SetMetadataFileExtension(IDbConnection conn, IDbTransaction tran)
        {
            var updatedMetadataFiles = new List<object>();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = tran;
                cmd.CommandText = "SELECT \"Id\", \"RelativePath\" FROM \"MetadataFiles\"";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var relativePath = reader.GetString(1);
                        var extension = relativePath.Substring(relativePath.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase));

                        updatedMetadataFiles.Add(new
                        {
                            Id = id,
                            Extension = extension
                        });
                    }
                }
            }

            var updateSql = $"UPDATE \"MetadataFiles\" SET \"Extension\" = @Extension WHERE \"Id\" = @Id";
            conn.Execute(updateSql, updatedMetadataFiles, transaction: tran);
        }
    }

    public class MetadataFile99
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public int? EditionFileId { get; set; }
        public int? BookNumber { get; set; }
        public string RelativePath { get; set; }
        public DateTime Added { get; set; }
        public DateTime LastUpdated { get; set; }
        public string Extension { get; set; }
        public string Hash { get; set; }
        public string Consumer { get; set; }
        public int Type { get; set; }
    }
}
