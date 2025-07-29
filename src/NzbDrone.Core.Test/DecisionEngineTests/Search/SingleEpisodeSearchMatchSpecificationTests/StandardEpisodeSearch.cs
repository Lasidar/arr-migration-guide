using System;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.DecisionEngine;
using NzbDrone.Core.DecisionEngine.Specifications.Search;
using NzbDrone.Core.IndexerSearch.Definitions;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Test.Common;

namespace NzbDrone.Core.Test.DecisionEngineTests.Search.SingleEpisodeSearchMatchSpecificationTests
{
    [TestFixture]
    public class StandardEpisodeSearch : TestBase<SingleEpisodeSearchMatchSpecification>
    {
        private RemoteEpisode _remoteEpisode = new();
        private SingleEpisodeSearchCriteria _searchCriteria = new();
        private ReleaseDecisionInformation _information;

        [SetUp]
        public void Setup()
        {
            _remoteEpisode.ParsedEpisodeInfo = new ParsedEpisodeInfo();
            _remoteEpisode.ParsedEpisodeInfo.BookNumber = 5;
            _remoteEpisode.ParsedEpisodeInfo.EditionNumbers = new[] { 1 };
            _remoteEpisode.MappedBookNumber = 5;

            _searchCriteria.BookNumber = 5;
            _searchCriteria.EditionNumber = 1;
            _information = new ReleaseDecisionInformation(false, _searchCriteria);
        }

        [Test]
        public void should_return_false_if_season_does_not_match()
        {
            _remoteEpisode.ParsedEpisodeInfo.BookNumber = 10;
            _remoteEpisode.MappedBookNumber = 10;

            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_return_true_if_season_matches_after_scenemapping()
        {
            _remoteEpisode.ParsedEpisodeInfo.BookNumber = 10;
            _remoteEpisode.MappedBookNumber = 5; // 10 -> 5 mapping
            _searchCriteria.BookNumber = 10; // searching by tvdb 5 = 10 scene

            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_return_false_if_season_does_not_match_after_scenemapping()
        {
            _remoteEpisode.ParsedEpisodeInfo.BookNumber = 10;
            _remoteEpisode.MappedBookNumber = 6; // 9 -> 5 mapping
            _searchCriteria.BookNumber = 9; // searching by tvdb 5 = 9 scene

            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_return_false_if_full_season_result_for_single_episode_search()
        {
            _remoteEpisode.ParsedEpisodeInfo.EditionNumbers = Array.Empty<int>();

            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_return_false_if_episode_number_does_not_match_search_criteria()
        {
            _remoteEpisode.ParsedEpisodeInfo.EditionNumbers = new[] { 2 };

            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_return_true_if_full_season_result_for_full_season_search()
        {
            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeTrue();
        }
    }
}
