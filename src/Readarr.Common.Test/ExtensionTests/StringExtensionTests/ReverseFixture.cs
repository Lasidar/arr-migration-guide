using FluentAssertions;
using NUnit.Framework;
using Readarr.Common.Extensions;

namespace Readarr.Common.Test.ExtensionTests.StringExtensionTests
{
    [TestFixture]
    public class ReverseFixture
    {
        [TestCase("input", "tupni")]
        [TestCase("racecar", "racecar")]
        public void should_reverse_string(string input, string expected)
        {
            input.Reverse().Should().Be(expected);
        }
    }
}
