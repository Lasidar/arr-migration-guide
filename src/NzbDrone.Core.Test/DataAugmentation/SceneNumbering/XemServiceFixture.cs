using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Core.DataAugmentation.Xem;
using NzbDrone.Core.DataAugmentation.Xem.Model;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;
using NzbDrone.Core.Books.Events;
using NzbDrone.Test.Common;

namespace NzbDrone.Core.Test.DataAugmentation.SceneNumbering
{
    [TestFixture]
    public class XemServiceFixture : CoreTest<XemService>
    {
        private Series _series;
        private List<int> _theXemAuthorIds;
        private List<XemSceneTvdbMapping> _theXemTvdbMappings;
        private List<Episode> _episodes;

        [SetUp]
        public void SetUp()
        {
            _series = Builder<Series>.CreateNew()
                .With(v => v.TvdbId = 10)
                .With(v => v.UseSceneNumbering = false)
                .BuildNew();

            _theXemAuthorIds = new List<int> { 120 };
            Mocker.GetMock<IXemProxy>()
                  .Setup(v => v.GetXemAuthorIds())
                  .Returns(_theXemAuthorIds);

            _theXemTvdbMappings = new List<XemSceneTvdbMapping>();
            Mocker.GetMock<IXemProxy>()
                  .Setup(v => v.GetSceneTvdbMappings(10))
                  .Returns(_theXemTvdbMappings);

            _episodes = new List<Episode>();
            _episodes.Add(new Episode { BookNumber = 1, EditionNumber = 1 });
            _episodes.Add(new Episode { BookNumber = 1, EditionNumber = 2 });
            _episodes.Add(new Episode { BookNumber = 2, EditionNumber = 1 });
            _episodes.Add(new Episode { BookNumber = 2, EditionNumber = 2 });
            _episodes.Add(new Episode { BookNumber = 2, EditionNumber = 3 });
            _episodes.Add(new Episode { BookNumber = 2, EditionNumber = 4 });
            _episodes.Add(new Episode { BookNumber = 2, EditionNumber = 5 });
            _episodes.Add(new Episode { BookNumber = 3, EditionNumber = 1 });
            _episodes.Add(new Episode { BookNumber = 3, EditionNumber = 2 });

            Mocker.GetMock<IEditionService>()
                  .Setup(v => v.GetEpisodeBySeries(It.IsAny<int>()))
                  .Returns(_episodes);
        }

        private void GivenTvdbMappings()
        {
            _theXemAuthorIds.Add(10);

            AddTvdbMapping(1, 1, 1, 1, 1, 1); // 1x01 -> 1x01
            AddTvdbMapping(2, 1, 2, 2, 1, 2); // 1x02 -> 1x02
            AddTvdbMapping(3, 2, 1, 3, 2, 1); // 2x01 -> 2x01
            AddTvdbMapping(4, 2, 2, 4, 2, 2); // 2x02 -> 2x02
            AddTvdbMapping(5, 2, 3, 5, 2, 3); // 2x03 -> 2x03
            AddTvdbMapping(6, 3, 1, 6, 2, 4); // 3x01 -> 2x04
            AddTvdbMapping(7, 3, 2, 7, 2, 5); // 3x02 -> 2x05
        }

        private void GivenExistingMapping()
        {
            _series.UseSceneNumbering = true;

            _episodes[0].SceneBookNumber = 1;
            _episodes[0].SceneEditionNumber = 1;
            _episodes[1].SceneBookNumber = 1;
            _episodes[1].SceneEditionNumber = 2;
            _episodes[2].SceneBookNumber = 2;
            _episodes[2].SceneEditionNumber = 1;
            _episodes[3].SceneBookNumber = 2;
            _episodes[3].SceneEditionNumber = 2;
            _episodes[4].SceneBookNumber = 2;
            _episodes[4].SceneEditionNumber = 3;
            _episodes[5].SceneBookNumber = 3;
            _episodes[5].SceneEditionNumber = 1;
            _episodes[6].SceneBookNumber = 3;
            _episodes[6].SceneEditionNumber = 1;
        }

        private void AddTvdbMapping(int sceneAbsolute, int sceneSeason, int sceneEpisode, int tvdbAbsolute, int tvdbSeason, int tvdbEpisode)
        {
            _theXemTvdbMappings.Add(new XemSceneTvdbMapping
            {
                Scene = new XemValues { Absolute = sceneAbsolute, Season = sceneSeason, Episode = sceneEpisode },
                Tvdb  = new XemValues { Absolute = tvdbAbsolute, Season = tvdbSeason, Episode = tvdbEpisode },
            });
        }

        [Test]
        public void should_not_fetch_scenenumbering_if_not_listed()
        {
            Subject.Handle(new SeriesUpdatedEvent(_series));

            Mocker.GetMock<IXemProxy>()
                  .Verify(v => v.GetSceneTvdbMappings(10), Times.Never());

            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.IsAny<Series>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());
        }

        [Test]
        public void should_fetch_scenenumbering()
        {
            GivenTvdbMappings();

            Subject.Handle(new SeriesUpdatedEvent(_series));

            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.Is<Series>(s => s.UseSceneNumbering == true), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [Test]
        public void should_clear_scenenumbering_if_removed_from_thexem()
        {
            GivenExistingMapping();

            Subject.Handle(new SeriesUpdatedEvent(_series));

            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.IsAny<Series>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());
        }

        [Test]
        public void should_not_clear_scenenumbering_if_no_results_at_all_from_thexem()
        {
            GivenExistingMapping();

            _theXemAuthorIds.Clear();

            Subject.Handle(new SeriesUpdatedEvent(_series));

            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.IsAny<Series>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());

            ExceptionVerification.ExpectedWarns(1);
        }

        [Test]
        public void should_not_clear_scenenumbering_if_thexem_throws()
        {
            GivenExistingMapping();

            Mocker.GetMock<IXemProxy>()
                  .Setup(v => v.GetXemAuthorIds())
                  .Throws(new InvalidOperationException());

            Subject.Handle(new SeriesUpdatedEvent(_series));

            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.IsAny<Series>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Never());

            ExceptionVerification.ExpectedWarns(1);
        }

        [Test]
        public void should_flag_unknown_future_episodes_if_existing_season_is_mapped()
        {
            GivenTvdbMappings();
            _theXemTvdbMappings.RemoveAll(v => v.Tvdb.Season == 2 && v.Tvdb.Episode == 5);

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 2 && v.EditionNumber == 5);

            episode.UnverifiedSceneNumbering.Should().BeTrue();
        }

        [Test]
        public void should_flag_unknown_future_season_if_future_season_is_shifted()
        {
            GivenTvdbMappings();

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 3 && v.EditionNumber == 1);

            episode.UnverifiedSceneNumbering.Should().BeTrue();
        }

        [Test]
        public void should_not_flag_unknown_future_season_if_future_season_is_not_shifted()
        {
            GivenTvdbMappings();
            _theXemTvdbMappings.RemoveAll(v => v.Scene.Season == 3);

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 3 && v.EditionNumber == 1);

            episode.UnverifiedSceneNumbering.Should().BeFalse();
        }

        [Test]
        public void should_not_flag_past_episodes_if_not_causing_overlaps()
        {
            GivenTvdbMappings();
            _theXemTvdbMappings.RemoveAll(v => v.Scene.Season == 2);

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 2 && v.EditionNumber == 1);

            episode.UnverifiedSceneNumbering.Should().BeFalse();
        }

        [Test]
        public void should_flag_past_episodes_if_causing_overlap()
        {
            GivenTvdbMappings();
            _theXemTvdbMappings.RemoveAll(v => v.Scene.Season == 2 && v.Tvdb.Episode <= 1);
            _theXemTvdbMappings.First(v => v.Scene.Season == 2 && v.Scene.Episode == 2).Scene.Episode = 1;

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 2 && v.EditionNumber == 1);

            episode.UnverifiedSceneNumbering.Should().BeTrue();
        }

        [Test]
        public void should_not_extrapolate_season_with_specials()
        {
            GivenTvdbMappings();
            var specialMapping = _theXemTvdbMappings.First(v => v.Tvdb.Season == 2 && v.Tvdb.Episode == 5);
            specialMapping.Tvdb.Season = 0;
            specialMapping.Tvdb.Episode = 1;

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 2 && v.EditionNumber == 5);

            episode.UnverifiedSceneNumbering.Should().BeTrue();
            episode.SceneBookNumber.Should().NotHaveValue();
            episode.SceneEditionNumber.Should().NotHaveValue();
        }

        [Test]
        public void should_extrapolate_season_with_future_episodes()
        {
            GivenTvdbMappings();
            _theXemTvdbMappings.RemoveAll(v => v.Tvdb.Season == 2 && v.Tvdb.Episode == 5);

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 2 && v.EditionNumber == 5);

            episode.UnverifiedSceneNumbering.Should().BeTrue();
            episode.SceneBookNumber.Should().Be(3);
            episode.SceneEditionNumber.Should().Be(2);
        }

        [Test]
        public void should_extrapolate_season_with_shifted_episodes()
        {
            GivenTvdbMappings();
            _theXemTvdbMappings.RemoveAll(v => v.Tvdb.Season == 2 && v.Tvdb.Episode == 5);
            var dualMapping = _theXemTvdbMappings.First(v => v.Tvdb.Season == 2 && v.Tvdb.Episode == 4);
            dualMapping.Scene.Season = 2;
            dualMapping.Scene.Episode = 3;

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 2 && v.EditionNumber == 5);

            episode.UnverifiedSceneNumbering.Should().BeTrue();
            episode.SceneBookNumber.Should().Be(2);
            episode.SceneEditionNumber.Should().Be(4);
        }

        [Test]
        public void should_extrapolate_shifted_future_seasons()
        {
            GivenTvdbMappings();

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 3 && v.EditionNumber == 2);

            episode.UnverifiedSceneNumbering.Should().BeTrue();
            episode.SceneBookNumber.Should().Be(4);
            episode.SceneEditionNumber.Should().Be(2);
        }

        [Test]
        public void should_not_extrapolate_matching_future_seasons()
        {
            GivenTvdbMappings();
            _theXemTvdbMappings.RemoveAll(v => v.Scene.Season != 1);

            Subject.Handle(new SeriesUpdatedEvent(_series));

            var episode = _episodes.First(v => v.BookNumber == 3 && v.EditionNumber == 2);

            episode.UnverifiedSceneNumbering.Should().BeFalse();
            episode.SceneBookNumber.Should().NotHaveValue();
            episode.SceneEditionNumber.Should().NotHaveValue();
        }

        [Test]
        public void should_skip_mapping_when_scene_information_is_all_zero()
        {
            GivenTvdbMappings();

            AddTvdbMapping(0, 0, 0, 8, 3, 1); // 3x01 -> 3x01
            AddTvdbMapping(0, 0, 0, 9, 3, 2); // 3x02 -> 3x02

            Subject.Handle(new SeriesUpdatedEvent(_series));

            Mocker.GetMock<IEditionService>()
                  .Verify(v => v.UpdateEpisodes(It.Is<List<Episode>>(e => e.Any(c => c.SceneAbsoluteEditionNumber == 0 && c.SceneBookNumber == 0 && c.SceneEditionNumber == 0))), Times.Never());
        }
    }
}
