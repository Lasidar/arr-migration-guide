using FluentMigrator;
using NzbDrone.Core.Datastore.Migration.Framework;

namespace NzbDrone.Core.Datastore.Migration
{
    [Migration(45)]
    public class add_indexes : NzbDroneMigrationBase
    {
        protected override void MainDbUpgrade()
        {
            Create.Index().OnTable("Blacklist").OnColumn("AuthorId");

            Create.Index().OnTable("EditionFiles").OnColumn("AuthorId");

            Create.Index().OnTable("Episodes").OnColumn("EditionFileId");
            Create.Index().OnTable("Episodes").OnColumn("AuthorId");

            Create.Index().OnTable("History").OnColumn("EpisodeId");
            Create.Index().OnTable("History").OnColumn("Date");

            Create.Index().OnTable("Series").OnColumn("Path");
            Create.Index().OnTable("Series").OnColumn("CleanTitle");
            Create.Index().OnTable("Series").OnColumn("TvRageId");
        }
    }
}
