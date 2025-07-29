using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.MetadataSource.SkyHook;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;
using NzbDrone.Test.Common;

namespace NzbDrone.Core.Test.TvTests
{
    [TestFixture]
    public class RefreshEditionServiceFixture : CoreTest<RefreshEditionService>
    {
        private List<Episode> _insertedEpisodes;
        private List<Episode> _updatedEpisodes;
        private List<Episode> _deletedEpisodes;
        private Tuple<Series, List<Episode>> _gameOfThrones;

        [OneTimeSetUp]
        public void TestFixture()
        {
            UseRealHttp();

            _gameOfThrones = Mocker.Resolve<SkyHookProxy>().GetSeriesInfo(121361); // Game of thrones

            // Remove specials.
            _gameOfThrones.Item2.RemoveAll(v => v.BookNumber == 0);
        }

        private List<Episode> GetEpisodes()
        {
            return _gameOfThrones.Item2.JsonClone();
        }

        private Series GetSeries()
        {
            var series = _gameOfThrones.Item1.JsonClone();

            return series;
        }

        private Series GetAnimeSeries()
        {
            var series = Builder<Series>.CreateNew().Build();
            series.SeriesType = SeriesTypes.Anime;

            return series;
        }

        [SetUp]
        public void Setup()
        {
            _insertedEpisodes = new List<Episode>();
            _updatedEpisodes = new List<Episode>();
            _deletedEpisodes = new List<Episode>();

            Mocker.GetMock<IEditionService>().Setup(c => c.InsertMany(It.IsAny<List<Episode>>()))
                .Callback<List<Episode>>(e => _insertedEpisodes = e);

            Mocker.GetMock<IEditionService>().Setup(c => c.UpdateMany(It.IsAny<List<Episode>>()))
                .Callback<List<Episode>>(e => _updatedEpisodes = e);

            Mocker.GetMock<IEditionService>().Setup(c => c.DeleteMany(It.IsAny<List<Episode>>()))
                .Callback<List<Episode>>(e => _deletedEpisodes = e);
        }

        [Test]
        public void should_create_all_when_no_existing_episodes()
        {
            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            Subject.RefreshEpisodeInfo(GetSeries(), GetEpisodes());

            _insertedEpisodes.Should().HaveSameCount(GetEpisodes());
            _updatedEpisodes.Should().BeEmpty();
            _deletedEpisodes.Should().BeEmpty();

            ExceptionVerification.ExpectedWarns(1);
        }

        [Test]
        public void should_update_all_when_all_existing_episodes()
        {
            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(GetEpisodes());

            Subject.RefreshEpisodeInfo(GetSeries(), GetEpisodes());

            _insertedEpisodes.Should().BeEmpty();
            _updatedEpisodes.Should().HaveSameCount(GetEpisodes());
            _deletedEpisodes.Should().BeEmpty();
        }

        [Test]
        public void should_delete_all_when_all_existing_episodes_are_gone_from_datasource()
        {
            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(GetEpisodes());

            Subject.RefreshEpisodeInfo(GetSeries(), new List<Episode>());

            _insertedEpisodes.Should().BeEmpty();
            _updatedEpisodes.Should().BeEmpty();
            _deletedEpisodes.Should().HaveSameCount(GetEpisodes());
        }

        [Test]
        public void should_delete_duplicated_episodes_based_on_season_episode_number()
        {
            var duplicateEpisodes = GetEpisodes().Skip(5).Take(2).ToList();

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(GetEpisodes().Union(duplicateEpisodes).ToList());

            Subject.RefreshEpisodeInfo(GetSeries(), GetEpisodes());

            _insertedEpisodes.Should().BeEmpty();
            _updatedEpisodes.Should().HaveSameCount(GetEpisodes());
            _deletedEpisodes.Should().HaveSameCount(duplicateEpisodes);
        }

        [Test]
        public void should_not_change_monitored_status_for_existing_episodes()
        {
            var series = GetSeries();
            series.Seasons = new List<Season>();
            series.Seasons.Add(new Season { BookNumber = 1, Monitored = false });

            var episodes = GetEpisodes();

            episodes.ForEach(e => e.Monitored = true);

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(episodes);

            Subject.RefreshEpisodeInfo(series, GetEpisodes());

            _updatedEpisodes.Should().HaveSameCount(GetEpisodes());
            _updatedEpisodes.Should().OnlyContain(e => e.Monitored == true);
        }

        [Test]
        public void should_not_set_monitored_status_for_old_episodes_to_false_if_episodes_existed()
        {
            var series = GetSeries();
            series.Seasons = new List<Season>();
            series.Seasons.Add(new Season { BookNumber = 1, Monitored = true });

            var episodes = GetEpisodes().OrderBy(v => v.BookNumber).ThenBy(v => v.EditionNumber).Take(5).ToList();

            episodes[1].AirDateUtc = DateTime.UtcNow.AddDays(-15);
            episodes[2].AirDateUtc = DateTime.UtcNow.AddDays(-10);
            episodes[3].AirDateUtc = DateTime.UtcNow.AddDays(1);

            var existingEpisodes = episodes.Skip(4).ToList();

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(existingEpisodes);

            Subject.RefreshEpisodeInfo(series, episodes);

            _insertedEpisodes = _insertedEpisodes.OrderBy(v => v.EditionNumber).ToList();

            _insertedEpisodes.Should().HaveCount(4);
            _insertedEpisodes[0].Monitored.Should().Be(true);
            _insertedEpisodes[1].Monitored.Should().Be(true);
            _insertedEpisodes[2].Monitored.Should().Be(true);
            _insertedEpisodes[3].Monitored.Should().Be(true);

            ExceptionVerification.ExpectedWarns(1);
        }

        [Test]
        public void should_remove_duplicate_remote_episodes_before_processing()
        {
            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            var episodes = Builder<Episode>.CreateListOfSize(5)
                                           .TheFirst(2)
                                           .With(e => e.BookNumber = 1)
                                           .With(e => e.EditionNumber = 1)
                                           .Build()
                                           .ToList();

            Subject.RefreshEpisodeInfo(GetSeries(), episodes);

            _insertedEpisodes.Should().HaveCount(episodes.Count - 1);
            _updatedEpisodes.Should().BeEmpty();
            _deletedEpisodes.Should().BeEmpty();
        }

        [Test]
        public void should_set_absolute_episode_number_for_anime()
        {
            var episodes = Builder<Episode>.CreateListOfSize(3).Build().ToList();

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            Subject.RefreshEpisodeInfo(GetAnimeSeries(), episodes);

            _insertedEpisodes.All(e => e.AbsoluteEditionNumber.HasValue).Should().BeTrue();
            _updatedEpisodes.Should().BeEmpty();
            _deletedEpisodes.Should().BeEmpty();
        }

        [Test]
        public void should_set_absolute_episode_number_even_if_not_previously_set_for_anime()
        {
            var episodes = Builder<Episode>.CreateListOfSize(3).Build().ToList();

            var existingEpisodes = episodes.JsonClone();
            existingEpisodes.ForEach(e => e.AbsoluteEditionNumber = null);

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(existingEpisodes);

            Subject.RefreshEpisodeInfo(GetAnimeSeries(), episodes);

            _insertedEpisodes.Should().BeEmpty();
            _updatedEpisodes.All(e => e.AbsoluteEditionNumber.HasValue).Should().BeTrue();
            _deletedEpisodes.Should().BeEmpty();
        }

        [Test]
        public void should_ignore_episodes_with_no_absolute_episode_in_distinct_by_absolute()
        {
            var episodes = Builder<Episode>.CreateListOfSize(10)
                                           .Build()
                                           .ToList();

            episodes[0].AbsoluteEditionNumber = null;
            episodes[1].AbsoluteEditionNumber = null;
            episodes[2].AbsoluteEditionNumber = null;
            episodes[3].AbsoluteEditionNumber = null;
            episodes[4].AbsoluteEditionNumber = null;

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            Subject.RefreshEpisodeInfo(GetAnimeSeries(), episodes);

            _insertedEpisodes.Should().HaveCount(episodes.Count);
        }

        [Test]
        public void should_override_empty_airdate_for_direct_to_dvd()
        {
            var series = GetSeries();
            series.Status = SeriesStatusType.Ended;

            var episodes = Builder<Episode>.CreateListOfSize(10)
                                           .All()
                                           .With(v => v.AirDateUtc = null)
                                           .BuildListOfNew();

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            List<Episode> updateEpisodes = null;
            Mocker.GetMock<IEditionService>().Setup(c => c.InsertMany(It.IsAny<List<Episode>>()))
                .Callback<List<Episode>>(c => updateEpisodes = c);

            Subject.RefreshEpisodeInfo(series, episodes);

            updateEpisodes.Should().NotBeNull();
            updateEpisodes.Should().NotBeEmpty();
            updateEpisodes.All(v => v.AirDateUtc.HasValue).Should().BeTrue();
        }

        [Test]
        public void should_use_tba_for_episode_title_when_null()
        {
            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            var episodes = Builder<Episode>.CreateListOfSize(1)
                                           .All()
                                           .With(e => e.Title = null)
                                           .Build()
                                           .ToList();

            Subject.RefreshEpisodeInfo(GetSeries(), episodes);

            _insertedEpisodes.First().Title.Should().Be("TBA");
        }

        [Test]
        public void should_update_air_date_when_multiple_episodes_air_on_the_same_day()
        {
            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            var now = DateTime.UtcNow;
            var series = GetSeries();

            var episodes = Builder<Episode>.CreateListOfSize(2)
                                           .All()
                                           .With(e => e.BookNumber = 1)
                                           .With(e => e.AirDate = now.ToShortDateString())
                                           .With(e => e.AirDateUtc = now)
                                           .Build()
                                           .ToList();

            Subject.RefreshEpisodeInfo(series, episodes);

            _insertedEpisodes.First().AirDateUtc.Value.ToString("s").Should().Be(episodes.First().AirDateUtc.Value.ToString("s"));
            _insertedEpisodes.Last().AirDateUtc.Value.ToString("s").Should().Be(episodes.First().AirDateUtc.Value.AddMinutes(series.Runtime).ToString("s"));
        }

        [Test]
        public void should_not_update_air_date_when_more_than_three_episodes_air_on_the_same_day()
        {
            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode>());

            var now = DateTime.UtcNow;
            var series = GetSeries();

            var episodes = Builder<Episode>.CreateListOfSize(4)
                                           .All()
                                           .With(e => e.BookNumber = 1)
                                           .With(e => e.AirDate = now.ToShortDateString())
                                           .With(e => e.AirDateUtc = now)
                                           .Build()
                                           .ToList();

            Subject.RefreshEpisodeInfo(series, episodes);

            _insertedEpisodes.Should().OnlyContain(e => e.AirDateUtc.Value.ToString("s") == episodes.First().AirDateUtc.Value.ToString("s"));
        }

        [Test]
        public void should_match_anime_episodes_by_season_and_episode_numbers()
        {
            var episodes = Builder<Episode>.CreateListOfSize(2)
                .Build()
                .ToList();

            episodes[0].AbsoluteEditionNumber = null;
            episodes[0].BookNumber.Should().NotBe(episodes[1].BookNumber);
            episodes[0].EditionNumber.Should().NotBe(episodes[1].EditionNumber);

            var existingEpisode = new Episode
            {
                BookNumber = episodes[0].BookNumber,
                EditionNumber = episodes[0].EditionNumber,
                AbsoluteEditionNumber = episodes[1].AbsoluteEditionNumber
            };

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode> { existingEpisode });

            Subject.RefreshEpisodeInfo(GetAnimeSeries(), episodes);

            _updatedEpisodes.First().BookNumber.Should().Be(episodes[0].BookNumber);
            _updatedEpisodes.First().EditionNumber.Should().Be(episodes[0].EditionNumber);
            _updatedEpisodes.First().AbsoluteEditionNumber.Should().Be(episodes[0].AbsoluteEditionNumber);

            _insertedEpisodes.First().BookNumber.Should().Be(episodes[1].BookNumber);
            _insertedEpisodes.First().EditionNumber.Should().Be(episodes[1].EditionNumber);
            _insertedEpisodes.First().AbsoluteEditionNumber.Should().Be(episodes[1].AbsoluteEditionNumber);
        }

        [Test]
        public void should_mark_updated_episodes_that_have_newly_added_absolute_episode_number()
        {
            var episodes = Builder<Episode>.CreateListOfSize(3)
                .Build()
                .ToList();

            var existingEpisodes = new List<Episode>
            {
                episodes[0],
                episodes[1]
            };

            existingEpisodes[0].AbsoluteEditionNumber = null;

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(existingEpisodes);

            Subject.RefreshEpisodeInfo(GetAnimeSeries(), episodes);

            _updatedEpisodes.First().BookNumber.Should().Be(episodes[1].BookNumber);
            _updatedEpisodes.First().EditionNumber.Should().Be(episodes[1].EditionNumber);
            _updatedEpisodes.First().AbsoluteEditionNumber.Should().NotBeNull();
            _updatedEpisodes.First().AbsoluteEditionNumberAdded.Should().BeTrue();

            _insertedEpisodes.Any(e => e.AbsoluteEditionNumberAdded).Should().BeFalse();
        }

        [Test]
        public void should_monitor_new_episode_if_season_is_monitored()
        {
            var series = GetSeries();
            series.Seasons = new List<Season>();
            series.Seasons.Add(new Season { BookNumber = 1, Monitored = true });

            var episodes = Builder<Episode>.CreateListOfSize(2)
                .All()
                .With(e => e.BookNumber = 1)
                .Build()
                .ToList();

            var existingEpisode = new Episode
            {
                BookNumber = episodes[0].BookNumber,
                EditionNumber = episodes[0].EditionNumber,
                Monitored = true
            };

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode> { existingEpisode });

            Subject.RefreshEpisodeInfo(series, episodes);

            _updatedEpisodes.Should().HaveCount(1);
            _insertedEpisodes.Should().HaveCount(1);
            _insertedEpisodes.Should().OnlyContain(e => e.Monitored == true);
        }

        [Test]
        public void should_not_monitor_new_episode_if_season_is_not_monitored()
        {
            var series = GetSeries();
            series.Seasons = new List<Season>();
            series.Seasons.Add(new Season { BookNumber = 1, Monitored = false });

            var episodes = Builder<Episode>.CreateListOfSize(2)
                .All()
                .With(e => e.BookNumber = 1)
                .Build()
                .ToList();

            var existingEpisode = new Episode
            {
                BookNumber = episodes[0].BookNumber,
                EditionNumber = episodes[0].EditionNumber,
                Monitored = true
            };

            Mocker.GetMock<IEditionService>().Setup(c => c.GetEpisodeBySeries(It.IsAny<int>()))
                .Returns(new List<Episode> { existingEpisode });

            Subject.RefreshEpisodeInfo(series, episodes);

            _updatedEpisodes.Should().HaveCount(1);
            _insertedEpisodes.Should().HaveCount(1);
            _insertedEpisodes.Should().OnlyContain(e => e.Monitored == false);
        }
    }
}
