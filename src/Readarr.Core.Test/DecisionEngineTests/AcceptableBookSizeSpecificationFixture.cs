using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Readarr.Common.Extensions;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.DecisionEngine.Specifications;
using Readarr.Core.Parser.Model;
using Readarr.Core.Profiles.Qualities;
using Readarr.Core.Qualities;
using Readarr.Core.Test.Framework;

namespace Readarr.Core.Test.DecisionEngineTests
{
    [TestFixture]
    public class AcceptableBookSizeSpecificationFixture : CoreTest<AcceptableBookSizeSpecification>
    {
        private RemoteBook _remoteBook;
        private Author _author;
        private Book _book;

        [SetUp]
        public void Setup()
        {
            _author = TestBuilders.CreateAuthor(1, "Test Author");
            _book = TestBuilders.CreateBook(1, "Test Book", _author);

            _author.QualityProfile = new LazyLoaded<QualityProfile>(new QualityProfile
            {
                Cutoff = Quality.EPUB.Id,
                Items = new List<QualityProfileQualityItem>
                {
                    new QualityProfileQualityItem
                    {
                        Quality = Quality.PDF,
                        MinSize = 1,
                        MaxSize = 10
                    },
                    new QualityProfileQualityItem
                    {
                        Quality = Quality.EPUB,
                        MinSize = 2,
                        MaxSize = 20
                    },
                    new QualityProfileQualityItem
                    {
                        Quality = Quality.AZW3,
                        MinSize = 5,
                        MaxSize = null
                    },
                    new QualityProfileQualityItem
                    {
                        Quality = Quality.MOBI,
                        MinSize = null,
                        MaxSize = 15
                    }
                }
            });

            _remoteBook = new RemoteBook
            {
                Author = _author,
                Books = new List<Book> { _book },
                Release = new ReleaseInfo(),
                Quality = new QualityModel(Quality.EPUB, new Revision(version: 1))
            };
        }

        [TestCase(5, true)]
        [TestCase(15, true)]
        [TestCase(1, false)]
        [TestCase(25, false)]
        public void should_return_expected_result_for_epub_quality(long size, bool expected)
        {
            _remoteBook.Release.Size = size.Megabytes();
            _remoteBook.Quality = new QualityModel(Quality.EPUB, new Revision(version: 1));

            Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation()).Accepted.Should().Be(expected);
        }

        [TestCase(2, true)]
        [TestCase(8, true)]
        [TestCase(0.5, false)]
        [TestCase(12, false)]
        public void should_return_expected_result_for_pdf_quality(double sizeInMb, bool expected)
        {
            _remoteBook.Release.Size = sizeInMb.Megabytes();
            _remoteBook.Quality = new QualityModel(Quality.PDF, new Revision(version: 1));

            Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation()).Accepted.Should().Be(expected);
        }

        [TestCase(6, true)]
        [TestCase(100, true)]
        [TestCase(4, false)]
        public void should_return_expected_result_for_azw3_quality(long size, bool expected)
        {
            _remoteBook.Release.Size = size.Megabytes();
            _remoteBook.Quality = new QualityModel(Quality.AZW3, new Revision(version: 1));

            Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation()).Accepted.Should().Be(expected);
        }

        [TestCase(1, true)]
        [TestCase(14, true)]
        [TestCase(20, false)]
        public void should_return_expected_result_for_mobi_quality(long size, bool expected)
        {
            _remoteBook.Release.Size = size.Megabytes();
            _remoteBook.Quality = new QualityModel(Quality.MOBI, new Revision(version: 1));

            Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation()).Accepted.Should().Be(expected);
        }

        [Test]
        public void should_return_true_if_size_is_zero()
        {
            _remoteBook.Release.Size = 0;
            Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation()).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_return_true_if_unlimited_max_size()
        {
            _remoteBook.Quality = new QualityModel(Quality.AZW3, new Revision(version: 1));
            _remoteBook.Release.Size = 1000.Megabytes();

            Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation()).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_reject_if_below_minimum_size()
        {
            _remoteBook.Quality = new QualityModel(Quality.EPUB, new Revision(version: 1));
            _remoteBook.Release.Size = 1.Megabytes();

            var result = Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation());
            
            result.Accepted.Should().BeFalse();
            result.RejectionReason.Should().Be(DownloadRejectionReason.BelowMinimumSize);
        }

        [Test]
        public void should_reject_if_above_maximum_size()
        {
            _remoteBook.Quality = new QualityModel(Quality.EPUB, new Revision(version: 1));
            _remoteBook.Release.Size = 25.Megabytes();

            var result = Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation());
            
            result.Accepted.Should().BeFalse();
            result.RejectionReason.Should().Be(DownloadRejectionReason.AboveMaximumSize);
        }

        [Test]
        public void should_handle_quality_groups()
        {
            var qualityProfile = new QualityProfile
            {
                Cutoff = Quality.EPUB.Id,
                Items = new List<QualityProfileQualityItem>
                {
                    new QualityProfileQualityItem
                    {
                        Id = 1001,
                        Name = "Ebook Formats",
                        Items = new List<QualityProfileQualityItem>
                        {
                            new QualityProfileQualityItem { Quality = Quality.EPUB, Allowed = true },
                            new QualityProfileQualityItem { Quality = Quality.MOBI, Allowed = true },
                            new QualityProfileQualityItem { Quality = Quality.AZW3, Allowed = true }
                        },
                        Allowed = true,
                        MinSize = 1,
                        MaxSize = 50
                    }
                }
            };

            _author.QualityProfile = new LazyLoaded<QualityProfile>(qualityProfile);
            _remoteBook.Quality = new QualityModel(Quality.EPUB, new Revision(version: 1));
            _remoteBook.Release.Size = 30.Megabytes();

            Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation()).Accepted.Should().BeTrue();
        }

        [Test]
        public void should_use_quality_group_size_when_available()
        {
            var qualityProfile = new QualityProfile
            {
                Cutoff = Quality.EPUB.Id,
                Items = new List<QualityProfileQualityItem>
                {
                    new QualityProfileQualityItem
                    {
                        Id = 1001,
                        Name = "Ebook Formats",
                        Items = new List<QualityProfileQualityItem>
                        {
                            new QualityProfileQualityItem { Quality = Quality.EPUB, Allowed = true },
                            new QualityProfileQualityItem { Quality = Quality.MOBI, Allowed = true }
                        },
                        Allowed = true,
                        MinSize = 5,
                        MaxSize = 15
                    }
                }
            };

            _author.QualityProfile = new LazyLoaded<QualityProfile>(qualityProfile);
            _remoteBook.Quality = new QualityModel(Quality.EPUB, new Revision(version: 1));
            _remoteBook.Release.Size = 20.Megabytes();

            var result = Subject.IsSatisfiedBy(_remoteBook, new ReleaseDecisionInformation());
            
            result.Accepted.Should().BeFalse();
            result.RejectionReason.Should().Be(DownloadRejectionReason.AboveMaximumSize);
        }
    }
}