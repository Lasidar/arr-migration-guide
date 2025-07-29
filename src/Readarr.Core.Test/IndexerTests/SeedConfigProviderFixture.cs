using System;
using System.Collections.Generic;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Readarr.Core.Indexers;
using Readarr.Core.Indexers.Torznab;
using Readarr.Core.Parser.Model;
using Readarr.Core.Test.Framework;

namespace Readarr.Core.Test.IndexerTests
{
    [TestFixture]
    public class SeedConfigProviderFixture : CoreTest<SeedConfigProvider>
    {
        [Test]
        public void should_not_return_config_for_non_existent_indexer()
        {
            Mocker.GetMock<ICachedIndexerSettingsProvider>()
                  .Setup(v => v.GetSettings(It.IsAny<int>()))
                  .Returns<CachedIndexerSettings>(null);

            var result = Subject.GetSeedConfiguration(new RemoteEpisode
            {
                Release = new ReleaseInfo
                {
                    DownloadProtocol = DownloadProtocol.Torrent,
                    IndexerId = 0
                }
            });

            result.Should().BeNull();
        }

        [Test]
        public void should_not_return_config_for_invalid_indexer()
        {
            Mocker.GetMock<ICachedIndexerSettingsProvider>()
                  .Setup(v => v.GetSettings(It.IsAny<int>()))
                  .Returns<CachedIndexerSettings>(null);

            var result = Subject.GetSeedConfiguration(new RemoteEpisode
            {
                Release = new ReleaseInfo
                {
                    DownloadProtocol = DownloadProtocol.Torrent,
                    IndexerId = 1
                },
                ParsedEpisodeInfo = new ParsedEpisodeInfo
                {
                    FullSeason = true
                }
            });

            result.Should().BeNull();
        }

        [Test]
        public void should_return_season_time_for_season_packs()
        {
            var settings = new TorznabSettings();
            settings.SeedCriteria.SeasonPackSeedTime = 10;

            Mocker.GetMock<ICachedIndexerSettingsProvider>()
                     .Setup(v => v.GetSettings(It.IsAny<int>()))
                     .Returns(new CachedIndexerSettings
                     {
                         FailDownloads = new HashSet<FailDownloads> { FailDownloads.Executables },
                         SeedCriteriaSettings = settings.SeedCriteria
                     });

            var result = Subject.GetSeedConfiguration(new RemoteEpisode
            {
                Release = new ReleaseInfo
                {
                    DownloadProtocol = DownloadProtocol.Torrent,
                    IndexerId = 1
                },
                ParsedEpisodeInfo = new ParsedEpisodeInfo
                {
                    FullSeason = true
                }
            });

            result.Should().NotBeNull();
            result.SeedTime.Should().Be(TimeSpan.FromMinutes(10));
        }
    }
}
