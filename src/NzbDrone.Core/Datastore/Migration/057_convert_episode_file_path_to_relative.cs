using System.Data;
using System.IO;
using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(57)]
    public class convert_episode_file_path_to_relative : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.Column("RelativePath").OnTable("EditionFiles").AsString().Nullable();

            // TODO: Add unique constraint for series ID and Relative Path
            // TODO: Warn if multiple series share the same path

            Execute.WithConnection(UpdateRelativePaths);
        }

        private void UpdateRelativePaths(IDbConnection conn, IDbTransaction tran)
        {
            using (var getSeriesCmd = conn.CreateCommand())
            {
                getSeriesCmd.Transaction = tran;
                getSeriesCmd.CommandText = "SELECT \"Id\", \"Path\" FROM \"Series\"";
                using (var seriesReader = getSeriesCmd.ExecuteReader())
                {
                    while (seriesReader.Read())
                    {
                        var seriesId = seriesReader.GetInt32(0);
                        var seriesPath = seriesReader.GetString(1) + Path.DirectorySeparatorChar;

                        using (var updateCmd = conn.CreateCommand())
                        {
                            updateCmd.Transaction = tran;
                            updateCmd.CommandText = "UPDATE \"EditionFiles\" SET \"RelativePath\" = REPLACE(\"Path\", ?, '') WHERE \"AuthorId\" = ?";
                            updateCmd.AddParameter(seriesPath);
                            updateCmd.AddParameter(seriesId);

                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
