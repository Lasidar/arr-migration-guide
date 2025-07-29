using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using Readarr.Core.DecisionEngine;
using Readarr.Core.DecisionEngine.Specifications.Search;
using Readarr.Core.IndexerSearch.Definitions;
using Readarr.Core.Parser.Model;
using Readarr.Core.Tv;
using Readarr.Test.Common;

namespace Readarr.Core.Test.DecisionEngineTests.Search
{
    [TestFixture]
    public class SeriesSpecificationFixture : TestBase<SeriesSpecification>
    {
        private Series _series1;
        private Series _series2;
        private RemoteEpisode _remoteEpisode = new();
        private SearchCriteriaBase _searchCriteria = new SingleEpisodeSearchCriteria();
        private ReleaseDecisionInformation _information;

        [SetUp]
        public void Setup()
        {
            _series1 = Builder<Series>.CreateNew().With(s => s.Id = 1).Build();
            _series2 = Builder<Series>.CreateNew().With(s => s.Id = 2).Build();

            _remoteEpisode.Series = _series1;
            _information = new ReleaseDecisionInformation(false, _searchCriteria);
        }

        [Test]
        public void should_return_false_if_series_doesnt_match()
        {
            _searchCriteria.Series = _series2;

            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeFalse();
        }

        [Test]
        public void should_return_true_when_series_ids_match()
        {
            _searchCriteria.Series = _series1;

            Subject.IsSatisfiedBy(_remoteEpisode, _information).Accepted.Should().BeTrue();
        }
    }
}
