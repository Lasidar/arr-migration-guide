using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Readarr.Core.Extras.Others;
using Readarr.Core.Housekeeping.Housekeepers;
using Readarr.Core.Languages;
using Readarr.Core.MediaFiles;
using Readarr.Core.Qualities;
using Readarr.Core.Test.Framework;
using Readarr.Core.Books;

namespace Readarr.Core.Test.Housekeeping.Housekeepers
{
    [TestFixture]
    public class CleanupOrphanedExtraFilesFixture : DbTest<CleanupOrphanedExtraFiles, OtherExtraFile>
    {
        [Test]
        public void should_delete_extra_files_that_dont_have_a_coresponding_series()
        {
            var episodeFile = Builder<EpisodeFile>.CreateNew()
                .With(h => h.Quality = new QualityModel())
                .With(h => h.Languages = new List<Language> { Language.English })
                .BuildNew();

            Db.Insert(episodeFile);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                    .With(m => m.EpisodeFileId = episodeFile.Id)
                                                    .BuildNew();

            Db.Insert(extraFile);
            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_not_delete_extra_files_that_have_a_coresponding_series()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            var episodeFile = Builder<EpisodeFile>.CreateNew()
                .With(h => h.Quality = new QualityModel())
                .With(h => h.Languages = new List<Language> { Language.English })
                .BuildNew();

            Db.Insert(series);
            Db.Insert(episodeFile);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                    .With(m => m.SeriesId = series.Id)
                                                    .With(m => m.EpisodeFileId = episodeFile.Id)
                                                    .BuildNew();

            Db.Insert(extraFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
        }

        [Test]
        public void should_delete_extra_files_that_dont_have_a_coresponding_episode_file()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            Db.Insert(series);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                    .With(m => m.SeriesId = series.Id)
                                                    .With(m => m.EpisodeFileId = 10)
                                                    .BuildNew();

            Db.Insert(extraFile);
            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_not_delete_extra_files_that_have_a_coresponding_episode_file()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            var episodeFile = Builder<EpisodeFile>.CreateNew()
                .With(h => h.Quality = new QualityModel())
                .With(h => h.Languages = new List<Language> { Language.English })
                .BuildNew();

            Db.Insert(series);
            Db.Insert(episodeFile);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                    .With(m => m.SeriesId = series.Id)
                                                    .With(m => m.EpisodeFileId = episodeFile.Id)
                                                    .BuildNew();

            Db.Insert(extraFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
        }

        [Test]
        public void should_delete_extra_files_that_have_episodefileid_of_zero()
        {
            var series = Builder<Series>.CreateNew()
                                        .BuildNew();

            Db.Insert(series);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                 .With(m => m.SeriesId = series.Id)
                                                 .With(m => m.EpisodeFileId = 0)
                                                 .BuildNew();

            Db.Insert(extraFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(0);
        }
    }
}
