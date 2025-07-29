using System.Collections.Generic;
using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.History;
using NzbDrone.Core.Housekeeping.Housekeepers;
using NzbDrone.Core.Languages;
using NzbDrone.Core.Qualities;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Test.Housekeeping.Housekeepers
{
    [TestFixture]
    public class CleanupOrphanedHistoryItemsFixture : DbTest<CleanupOrphanedHistoryItems, EpisodeHistory>
    {
        private Series _series;
        private Episode _episode;

        [SetUp]
        public void Setup()
        {
            _series = Builder<Series>.CreateNew()
                                     .BuildNew();

            _episode = Builder<Episode>.CreateNew()
                                       .BuildNew();
        }

        private void GivenSeries()
        {
            Db.Insert(_series);
        }

        private void GivenEpisode()
        {
            Db.Insert(_episode);
        }

        [Test]
        public void should_delete_orphaned_items_by_series()
        {
            GivenEpisode();

            var history = Builder<EpisodeHistory>.CreateNew()
                .With(h => h.Languages = new List<Language> { Language.English })
                .With(h => h.Quality = new QualityModel())
                .With(h => h.EpisodeId = _episode.Id)
                .BuildNew();
            Db.Insert(history);

            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_delete_orphaned_items_by_episode()
        {
            GivenSeries();

            var history = Builder<EpisodeHistory>.CreateNew()
                .With(h => h.Languages = new List<Language> { Language.English })
                .With(h => h.Quality = new QualityModel())
                .With(h => h.AuthorId = _series.Id)
                .BuildNew();
            Db.Insert(history);

            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_not_delete_unorphaned_data_by_series()
        {
            GivenSeries();
            GivenEpisode();

            var history = Builder<EpisodeHistory>.CreateListOfSize(2)
                .All()
                .With(h => h.Languages = new List<Language> { Language.English })
                .With(h => h.Quality = new QualityModel())
                .With(h => h.EpisodeId = _episode.Id)
                .TheFirst(1)
                .With(h => h.AuthorId = _series.Id)
                .BuildListOfNew();

            Db.InsertMany(history);

            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
            AllStoredModels.Should().Contain(h => h.AuthorId == _series.Id);
        }

        [Test]
        public void should_not_delete_unorphaned_data_by_episode()
        {
            GivenSeries();
            GivenEpisode();

            var history = Builder<EpisodeHistory>.CreateListOfSize(2)
                .All()
                .With(h => h.Languages = new List<Language> { Language.English })
                .With(h => h.Quality = new QualityModel())
                .With(h => h.AuthorId = _series.Id)
                .TheFirst(1)
                .With(h => h.EpisodeId = _episode.Id)
                .BuildListOfNew();

            Db.InsertMany(history);

            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
            AllStoredModels.Should().Contain(h => h.EpisodeId == _episode.Id);
        }
    }
}
