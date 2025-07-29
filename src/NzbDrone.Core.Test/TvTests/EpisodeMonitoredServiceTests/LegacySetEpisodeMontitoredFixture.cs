using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Books;

namespace NzbDrone.Core.Test.TvTests.EpisodeMonitoredServiceTests
{
    [TestFixture]
    public class LegacySetEpisodeMontitoredFixture : CoreTest<EpisodeMonitoredService>
    {
        private Series _series;
        private List<Episode> _episodes;

        [SetUp]
        public void Setup()
        {
            var seasons = 4;

            _series = Builder<Series>.CreateNew()
                                     .With(s => s.Seasons = Builder<Season>.CreateListOfSize(seasons)
                                                                           .All()
                                                                           .With(n => n.Monitored = true)
                                                                           .Build()
                                                                           .ToList())
                                     .Build();

            _episodes = Builder<Episode>.CreateListOfSize(seasons)
                                        .All()
                                        .With(e => e.Monitored = true)
                                        .With(e => e.AirDateUtc = DateTime.UtcNow.AddDays(-7))

                                        // Missing
                                        .TheFirst(1)
                                        .With(e => e.EditionFileId = 0)

                                        // Has File
                                        .TheNext(1)
                                        .With(e => e.EditionFileId = 1)

                                         // Future
                                        .TheNext(1)
                                        .With(e => e.EditionFileId = 0)
                                        .With(e => e.AirDateUtc = DateTime.UtcNow.AddDays(7))

                                        // Future/TBA
                                        .TheNext(1)
                                        .With(e => e.EditionFileId = 0)
                                        .With(e => e.AirDateUtc = null)
                                        .Build()
                                        .ToList();

            Mocker.GetMock<IEditionService>()
                  .Setup(s => s.GetEpisodeBySeries(It.IsAny<int>()))
                  .Returns(_episodes);
        }

        private void GivenSpecials()
        {
            foreach (var episode in _episodes)
            {
                episode.BookNumber = 0;
            }

            _series.Seasons = new List<Season> { new Season { Monitored = false, BookNumber = 0 } };
        }

        [Test]
        public void should_be_able_to_monitor_series_without_changing_episodes()
        {
            Subject.SetEpisodeMonitoredStatus(_series, null);

            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.IsAny<Series>(), It.IsAny<bool>(), It.IsAny<bool>()), Times.Once());

            Mocker.GetMock<IEditionService>()
                  .Verify(v => v.UpdateEpisodes(It.IsAny<List<Episode>>()), Times.Never());
        }

        [Test]
        public void should_be_able_to_monitor_all_episodes()
        {
            Subject.SetEpisodeMonitoredStatus(_series, new MonitoringOptions());

            Mocker.GetMock<IEditionService>()
                  .Verify(v => v.UpdateEpisodes(It.Is<List<Episode>>(l => l.All(e => e.Monitored))));
        }

        [Test]
        public void should_be_able_to_monitor_missing_episodes_only()
        {
            var monitoringOptions = new MonitoringOptions
                                    {
                                        IgnoreEpisodesWithFiles = true,
                                        IgnoreEpisodesWithoutFiles = false
                                    };

            Subject.SetEpisodeMonitoredStatus(_series, monitoringOptions);

            VerifyMonitored(e => !e.HasFile);
            VerifyNotMonitored(e => e.HasFile);
        }

        [Test]
        public void should_be_able_to_monitor_new_episodes_only()
        {
            var monitoringOptions = new MonitoringOptions
            {
                IgnoreEpisodesWithFiles = true,
                IgnoreEpisodesWithoutFiles = true
            };

            Subject.SetEpisodeMonitoredStatus(_series, monitoringOptions);

            VerifyMonitored(e => e.AirDateUtc.HasValue && e.AirDateUtc.Value.After(DateTime.UtcNow));
            VerifyMonitored(e => !e.AirDateUtc.HasValue);
            VerifyNotMonitored(e => e.AirDateUtc.HasValue && e.AirDateUtc.Value.Before(DateTime.UtcNow));
        }

        [Test]
        public void should_not_monitor_missing_specials()
        {
            GivenSpecials();

            var monitoringOptions = new MonitoringOptions
            {
                IgnoreEpisodesWithFiles = true,
                IgnoreEpisodesWithoutFiles = false
            };

            Subject.SetEpisodeMonitoredStatus(_series, monitoringOptions);

            VerifyNotMonitored(e => e.BookNumber == 0);
        }

        [Test]
        public void should_not_monitor_new_specials()
        {
            GivenSpecials();

            var monitoringOptions = new MonitoringOptions
            {
                IgnoreEpisodesWithFiles = true,
                IgnoreEpisodesWithoutFiles = true
            };

            Subject.SetEpisodeMonitoredStatus(_series, monitoringOptions);

            VerifyNotMonitored(e => e.BookNumber == 0);
        }

        [Test]
        public void should_not_monitor_season_when_all_episodes_are_monitored_except_latest_season()
        {
            _series.Seasons = Builder<Season>.CreateListOfSize(2)
                                             .All()
                                             .With(n => n.Monitored = true)
                                             .Build()
                                             .ToList();

            _episodes = Builder<Episode>.CreateListOfSize(5)
                                        .All()
                                        .With(e => e.BookNumber = 1)
                                        .With(e => e.EditionFileId = 0)
                                        .With(e => e.AirDateUtc = DateTime.UtcNow.AddDays(-5))
                                        .TheLast(1)
                                        .With(e => e.BookNumber = 2)
                                        .Build()
                                        .ToList();

            Mocker.GetMock<IEditionService>()
                  .Setup(s => s.GetEpisodeBySeries(It.IsAny<int>()))
                  .Returns(_episodes);

            var monitoringOptions = new MonitoringOptions
            {
                IgnoreEpisodesWithoutFiles = true
            };

            Subject.SetEpisodeMonitoredStatus(_series, monitoringOptions);

            VerifySeasonMonitored(n => n.BookNumber == 2);
            VerifySeasonNotMonitored(n => n.BookNumber == 1);
        }

        [Test]
        public void should_ignore_episodes_when_season_is_not_monitored()
        {
            _series.Seasons.ForEach(s => s.Monitored = false);

            Subject.SetEpisodeMonitoredStatus(_series, new MonitoringOptions());

            Mocker.GetMock<IEditionService>()
                  .Verify(v => v.UpdateEpisodes(It.Is<List<Episode>>(l => l.All(e => !e.Monitored))));
        }

        [Test]
        public void should_should_not_monitor_episodes_if_season_is_not_monitored()
        {
            _series = Builder<Series>.CreateNew()
                                     .With(s => s.Seasons = Builder<Season>.CreateListOfSize(2)
                                                                           .TheFirst(1)
                                                                           .With(n => n.Monitored = true)
                                                                           .TheLast(1)
                                                                           .With(n => n.Monitored = false)
                                                                           .Build()
                                                                           .ToList())
                                     .Build();

            var episodes = Builder<Episode>.CreateListOfSize(10)
                                           .All()
                                           .With(e => e.Monitored = true)
                                           .With(e => e.EditionFileId = 0)
                                           .With(e => e.AirDateUtc = DateTime.UtcNow.AddDays(-7))
                                           .TheFirst(5)
                                           .With(e => e.BookNumber = 1)
                                           .TheLast(5)
                                           .With(e => e.BookNumber = 2)
                                           .BuildList();

            Mocker.GetMock<IEditionService>()
                  .Setup(s => s.GetEpisodeBySeries(It.IsAny<int>()))
                  .Returns(episodes);

            Subject.SetEpisodeMonitoredStatus(_series, new MonitoringOptions
                                                       {
                                                           IgnoreEpisodesWithFiles = true,
                                                           IgnoreEpisodesWithoutFiles = false
                                                       });

            VerifyMonitored(e => e.BookNumber == 1);
            VerifyNotMonitored(e => e.BookNumber == 2);
            VerifySeasonMonitored(s => s.BookNumber == 1);
            VerifySeasonNotMonitored(s => s.BookNumber == 2);
        }

        private void VerifyMonitored(Func<Episode, bool> predicate)
        {
            Mocker.GetMock<IEditionService>()
                .Verify(v => v.UpdateEpisodes(It.Is<List<Episode>>(l => l.Where(predicate).All(e => e.Monitored))));
        }

        private void VerifyNotMonitored(Func<Episode, bool> predicate)
        {
            Mocker.GetMock<IEditionService>()
                .Verify(v => v.UpdateEpisodes(It.Is<List<Episode>>(l => l.Where(predicate).All(e => !e.Monitored))));
        }

        private void VerifySeasonMonitored(Func<Season, bool> predicate)
        {
            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.Is<Series>(s => s.Seasons.Where(predicate).All(n => n.Monitored)), It.IsAny<bool>(), It.IsAny<bool>()));
        }

        private void VerifySeasonNotMonitored(Func<Season, bool> predicate)
        {
            Mocker.GetMock<IAuthorService>()
                  .Verify(v => v.UpdateSeries(It.Is<Series>(s => s.Seasons.Where(predicate).All(n => !n.Monitored)), It.IsAny<bool>(), It.IsAny<bool>()));
        }
    }
}
