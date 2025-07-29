using FluentAssertions;
using NUnit.Framework;
using Readarr.Core.Test.Framework;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Test.ThingiProviderTests
{
    [TestFixture]
    public class NullConfigFixture : CoreTest<NullConfig>
    {
        [Test]
        public void should_be_valid()
        {
            Subject.Validate().IsValid.Should().BeTrue();
        }
    }
}
