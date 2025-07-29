using FluentAssertions;
using NUnit.Framework;
using Readarr.Core.Datastore.Migration;
using Readarr.Core.Datastore.Migration.Framework;
using Readarr.Core.Test.Framework;

namespace Readarr.Core.Test.Datastore.Migration
{
    [Ignore("not sure for now")]
    [TestFixture]
    public class rename_quality_profiles_add_upgrade_allowedFixture : MigrationTest<rename_quality_profiles_add_upgrade_allowed>
    {
        [Test]
        public void should_handle_injected_radarr_migration()
        {
            var dbBefore = WithTestDb(new MigrationContext(MigrationType, 110));

            // Ensure 111 isn't applied
            dbBefore.GetDirectDataMapper().Query("INSERT INTO \"VersionInfo\" (\"Version\", \"AppliedOn\", \"Description\") VALUES (111, '2018-12-24T18:21:07', 'remove_bitmetv')");

            var dbAfter = WithMigrationTestDb();

            var result = dbAfter.QueryScalar<int>("SELECT COUNT(*) FROM \"VersionInfo\" WHERE \"Description\" = 'remove_bitmetv'");

            result.Should().Be(0);
        }
    }
}
