using FluentAssertions;
using NUnit.Framework;
using Readarr.Common.EnvironmentInfo;
using Readarr.Common.Http;
using Readarr.Test.Common;

namespace Readarr.Common.Test.Http
{
    [TestFixture]
    public class UserAgentBuilderFixture : TestBase<UserAgentBuilder>
    {
        [Test]
        public void should_get_user_agent_if_os_version_is_null()
        {
            Mocker.GetMock<IOsInfo>().SetupGet(c => c.Version).Returns((string)null);
            Mocker.GetMock<IOsInfo>().SetupGet(c => c.Name).Returns("TestOS");

            Subject.GetUserAgent(false).Should().NotBeNullOrWhiteSpace();
        }

        [Test]
        public void should_get_use_os_family_if_name_is_null()
        {
            Mocker.GetMock<IOsInfo>().SetupGet(c => c.Version).Returns((string)null);
            Mocker.GetMock<IOsInfo>().SetupGet(c => c.Name).Returns((string)null);

            Subject.GetUserAgent(false).Should().NotBeNullOrWhiteSpace();
        }
    }
}
