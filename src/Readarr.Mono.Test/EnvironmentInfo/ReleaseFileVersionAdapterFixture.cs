using FluentAssertions;
using NUnit.Framework;
using Readarr.Common.Disk;
using Readarr.Mono.Disk;
using Readarr.Mono.EnvironmentInfo.VersionAdapters;
using Readarr.Test.Common;

namespace Readarr.Mono.Test.EnvironmentInfo
{
    [TestFixture]
    [Platform("Linux")]
    public class ReleaseFileVersionAdapterFixture : TestBase<ReleaseFileVersionAdapter>
    {
        [SetUp]
        public void Setup()
        {
            NotBsd();

            Mocker.SetConstant<IDiskProvider>(Mocker.Resolve<DiskProvider>());
        }

        [Test]
        public void should_get_version_info()
        {
            var info = Subject.Read();
            info.FullName.Should().NotBeNullOrWhiteSpace();
            info.Name.Should().NotBeNullOrWhiteSpace();
            info.Version.Should().NotBeNullOrWhiteSpace();
        }
    }
}
