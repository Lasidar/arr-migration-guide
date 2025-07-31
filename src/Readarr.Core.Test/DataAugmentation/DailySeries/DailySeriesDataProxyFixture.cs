using FluentAssertions;
using NUnit.Framework;
using Readarr.Core.DataAugmentation.DailySeries;
using Readarr.Core.Test.Framework;
using Readarr.Test.Common.Categories;
using Readarr.Core.Tv;

namespace Readarr.Core.Test.DataAugmentation.DailySeries
{
    [TestFixture]
    [IntegrationTest]
    public class DailySeriesDataProxyFixture : CoreTest<DailySeriesDataProxy>
    {
        [SetUp]
        public void Setup()
        {
            UseRealHttp();
        }

        [Test]
        public void should_get_list_of_daily_series()
        {
            var list = Subject.GetDailySeriesIds();
            list.Should().NotBeEmpty();
            list.Should().OnlyHaveUniqueItems();
        }
    }
}
