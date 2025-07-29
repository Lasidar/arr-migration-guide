using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Extras.Others;
using NzbDrone.Core.Housekeeping.Housekeepers;
using NzbDrone.Core.Languages;
using NzbDrone.Core.MediaFiles;
using NzbDrone.Core.Qualities;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Test.Housekeeping.Housekeepers
{
    [TestFixture]
    public class CleanupOrphanedExtraFilesFixture : DbTest<CleanupOrphanedExtraFiles, OtherExtraFile>
    {
        [Test]
        public void should_delete_extra_files_that_dont_have_a_coresponding_series()
        {
            var episodeFile = Builder<EditionFile>.CreateNew()
                .With(h => h.Quality = new QualityModel())
                .With(h => h.Languages = new List<Language> { Language.English })
                .BuildNew();

            Db.Insert(episodeFile);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                    .With(m => m.EditionFileId = episodeFile.Id)
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

            var episodeFile = Builder<EditionFile>.CreateNew()
                .With(h => h.Quality = new QualityModel())
                .With(h => h.Languages = new List<Language> { Language.English })
                .BuildNew();

            Db.Insert(series);
            Db.Insert(episodeFile);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                    .With(m => m.AuthorId = series.Id)
                                                    .With(m => m.EditionFileId = episodeFile.Id)
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
                                                    .With(m => m.AuthorId = series.Id)
                                                    .With(m => m.EditionFileId = 10)
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

            var episodeFile = Builder<EditionFile>.CreateNew()
                .With(h => h.Quality = new QualityModel())
                .With(h => h.Languages = new List<Language> { Language.English })
                .BuildNew();

            Db.Insert(series);
            Db.Insert(episodeFile);

            var extraFile = Builder<OtherExtraFile>.CreateNew()
                                                    .With(m => m.AuthorId = series.Id)
                                                    .With(m => m.EditionFileId = episodeFile.Id)
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
                                                 .With(m => m.AuthorId = series.Id)
                                                 .With(m => m.EditionFileId = 0)
                                                 .BuildNew();

            Db.Insert(extraFile);
            Subject.Clean();
            AllStoredModels.Should().HaveCount(0);
        }
    }
}
