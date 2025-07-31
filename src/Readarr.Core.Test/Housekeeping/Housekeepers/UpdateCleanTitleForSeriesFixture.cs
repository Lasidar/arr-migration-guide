using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using Readarr.Core.Housekeeping.Housekeepers;
using Readarr.Core.Test.Framework;
using Readarr.Core.Books;

namespace Readarr.Core.Test.Housekeeping.Housekeepers
{
    [TestFixture]
    public class UpdateCleanTitleForSeriesFixture : CoreTest<UpdateCleanTitleForSeries>
    {
        [Test]
        public void should_update_clean_title()
        {
            var series = Builder<Series>.CreateNew()
                                        .With(s => s.Title = "Full Title")
                                        .With(s => s.CleanTitle = "unclean")
                                        .Build();

            Mocker.GetMock<ISeriesRepository>()
                 .Setup(s => s.All())
                 .Returns(new[] { series });

            Subject.Clean();

            Mocker.GetMock<ISeriesRepository>()
                .Verify(v => v.Update(It.Is<Series>(s => s.CleanTitle == "fulltitle")), Times.Once());
        }

        [Test]
        public void should_not_update_unchanged_title()
        {
            var series = Builder<Series>.CreateNew()
                                        .With(s => s.Title = "Full Title")
                                        .With(s => s.CleanTitle = "fulltitle")
                                        .Build();

            Mocker.GetMock<ISeriesRepository>()
                 .Setup(s => s.All())
                 .Returns(new[] { series });

            Subject.Clean();

            Mocker.GetMock<ISeriesRepository>()
                .Verify(v => v.Update(It.Is<Series>(s => s.CleanTitle == "fulltitle")), Times.Never());
        }
    }
}
