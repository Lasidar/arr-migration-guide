using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Readarr.Core.Books;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.Parser.Model;
using Readarr.Core.Profiles.Qualities;
using Readarr.Core.Qualities;
using Readarr.Core.Test.Framework;
using System.Collections.Generic;

namespace Readarr.Core.Test.DecisionEngineTests
{
    [TestFixture]
    public class AcceptableBookSizeSpecificationFixture : CoreTest<AcceptableBookSizeSpecification>
    {
        private RemoteBook _remoteBook;
        private Author _author;
        private QualityProfile _qualityProfile;

        [SetUp]
        public void Setup()
        {
            _author = TestBuilders.CreateAuthor();
            
            _qualityProfile = new QualityProfile
            {
                Items = new List<QualityProfileQualityItem>
                {
                    new QualityProfileQualityItem
                    {
                        Quality = Quality.EPUB,
                        MinSize = 0.5,  // 0.5 MB
                        MaxSize = 50    // 50 MB
                    },
                    new QualityProfileQualityItem
                    {
                        Quality = Quality.PDF,
                        MinSize = 1,    // 1 MB
                        MaxSize = 100   // 100 MB
                    }
                }
            };

            _author.QualityProfile = new LazyLoaded<QualityProfile>(_qualityProfile);

            _remoteBook = new RemoteBook
            {
                Author = _author,
                Books = new List<Book> { TestBuilders.CreateBook(1, "Test Book", _author) },
                Quality = new QualityModel(Quality.EPUB),
                Release = Builder<ReleaseInfo>.CreateNew()
                    .With(r => r.Size = 10 * 1024 * 1024) // 10 MB
                    .Build()
            };
        }

        [Test]
        public void should_accept_if_size_is_within_limits()
        {
            _remoteBook.Release.Size = 10 * 1024 * 1024; // 10 MB

            Subject.IsSatisfiedBy(_remoteBook, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_reject_if_size_is_below_minimum()
        {
            _remoteBook.Release.Size = 100 * 1024; // 100 KB

            var result = Subject.IsSatisfiedBy(_remoteBook, null);
            
            result.Accepted.Should().BeFalse();
            result.Rejections[0].Type.Should().Be(RejectionType.Permanent);
            result.Rejections[0].Reason.Should().Contain("smaller than minimum allowed");
        }

        [Test]
        public void should_reject_if_size_is_above_maximum()
        {
            _remoteBook.Release.Size = 100 * 1024 * 1024; // 100 MB

            var result = Subject.IsSatisfiedBy(_remoteBook, null);
            
            result.Accepted.Should().BeFalse();
            result.Rejections[0].Type.Should().Be(RejectionType.Permanent);
            result.Rejections[0].Reason.Should().Contain("larger than maximum allowed");
        }

        [Test]
        public void should_accept_if_size_is_zero()
        {
            _remoteBook.Release.Size = 0;

            Subject.IsSatisfiedBy(_remoteBook, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_use_quality_specific_limits()
        {
            _remoteBook.Quality = new QualityModel(Quality.PDF);
            _remoteBook.Release.Size = 80 * 1024 * 1024; // 80 MB

            // This would be rejected for EPUB (max 50MB) but accepted for PDF (max 100MB)
            Subject.IsSatisfiedBy(_remoteBook, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_accept_if_no_size_limits_set()
        {
            _qualityProfile.Items[0].MinSize = null;
            _qualityProfile.Items[0].MaxSize = null;

            Subject.IsSatisfiedBy(_remoteBook, null).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_accept_if_max_size_is_unlimited()
        {
            _qualityProfile.Items[0].MaxSize = 0;
            _remoteBook.Release.Size = 1000 * 1024 * 1024; // 1 GB

            Subject.IsSatisfiedBy(_remoteBook, null).Accepted.Should().BeTrue();
        }
    }
}