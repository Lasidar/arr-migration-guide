using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using Readarr.Core.Configuration;
using Readarr.Core.ImportLists;
using Readarr.Core.ImportLists.Goodreads;
using Readarr.Core.Parser.Model;
using Readarr.Core.Test.Framework;

namespace Readarr.Core.Test.ImportListTests
{
    [TestFixture]
    public class GoodreadsListImportFixture : CoreTest<GoodreadsListImport>
    {
        private GoodreadsListSettings _settings;

        [SetUp]
        public void Setup()
        {
            _settings = new GoodreadsListSettings
            {
                ListId = "123456",
                ApiKey = "test-api-key"
            };

            Subject.Definition = new ImportListDefinition
            {
                Id = 1,
                Name = "Test Goodreads List",
                Settings = _settings
            };
        }

        [Test]
        public void should_fetch_books_from_goodreads()
        {
            var goodreadsBooks = new List<GoodreadsBook>
            {
                new GoodreadsBook
                {
                    Title = "Test Book 1",
                    AuthorName = "Test Author 1",
                    GoodreadsId = "111111",
                    Isbn = "9781234567890",
                    PublicationYear = 2020,
                    Publisher = "Test Publisher"
                },
                new GoodreadsBook
                {
                    Title = "Test Book 2",
                    AuthorName = "Test Author 2",
                    GoodreadsId = "222222",
                    Isbn = "9780987654321",
                    PublicationYear = 2021,
                    Publisher = "Another Publisher"
                }
            };

            Mocker.GetMock<IGoodreadsProxy>()
                .Setup(s => s.GetListBooks(_settings.ListId, _settings.ApiKey))
                .Returns(goodreadsBooks);

            var result = Subject.Fetch();

            result.Books.Should().HaveCount(2);
            result.AnyFailure.Should().BeFalse();
            
            var firstBook = result.Books.First();
            firstBook.Title.Should().Be("Test Book 1");
            firstBook.AuthorName.Should().Be("Test Author 1");
            firstBook.GoodreadsId.Should().Be("111111");
            firstBook.Isbn.Should().Be("9781234567890");
            firstBook.Year.Should().Be(2020);
            firstBook.Publisher.Should().Be("Test Publisher");
        }

        [Test]
        public void should_handle_goodreads_api_failure()
        {
            Mocker.GetMock<IGoodreadsProxy>()
                .Setup(s => s.GetListBooks(_settings.ListId, _settings.ApiKey))
                .Throws(new Exception("API Error"));

            var result = Subject.Fetch();

            result.Books.Should().BeEmpty();
            result.AnyFailure.Should().BeTrue();

            Mocker.GetMock<IImportListStatusService>()
                .Verify(v => v.RecordFailure(1), Times.Once());
        }

        [Test]
        public void should_clean_duplicate_books()
        {
            var goodreadsBooks = new List<GoodreadsBook>
            {
                new GoodreadsBook
                {
                    Title = "Test Book",
                    AuthorName = "Test Author",
                    GoodreadsId = "111111",
                    Isbn = "9781234567890"
                },
                new GoodreadsBook
                {
                    Title = "Test Book", // Duplicate
                    AuthorName = "Test Author",
                    GoodreadsId = "111111",
                    Isbn = "9781234567890"
                }
            };

            Mocker.GetMock<IGoodreadsProxy>()
                .Setup(s => s.GetListBooks(_settings.ListId, _settings.ApiKey))
                .Returns(goodreadsBooks);

            var result = Subject.Fetch();

            result.Books.Should().HaveCount(1);
        }

        [Test]
        public void should_set_import_list_info_on_books()
        {
            var goodreadsBooks = new List<GoodreadsBook>
            {
                new GoodreadsBook
                {
                    Title = "Test Book",
                    AuthorName = "Test Author",
                    GoodreadsId = "111111"
                }
            };

            Mocker.GetMock<IGoodreadsProxy>()
                .Setup(s => s.GetListBooks(_settings.ListId, _settings.ApiKey))
                .Returns(goodreadsBooks);

            var result = Subject.Fetch();

            var book = result.Books.First();
            book.ImportListId.Should().Be(1);
            book.ImportList.Should().Be("Test Goodreads List");
        }

        [Test]
        public void should_validate_settings()
        {
            var validationResult = new ValidationFailure("ApiKey", "Invalid API Key");

            Mocker.GetMock<IGoodreadsProxy>()
                .Setup(s => s.Test(_settings))
                .Returns(validationResult);

            var result = Subject.Test();

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "ApiKey");
        }

        [Test]
        public void should_have_minimum_refresh_interval()
        {
            Subject.MinRefreshInterval.Should().Be(TimeSpan.FromHours(6));
        }

        [Test]
        public void should_have_correct_list_type()
        {
            Subject.ListType.Should().Be(ImportListType.Goodreads);
        }

        [Test]
        public void should_handle_books_with_missing_data()
        {
            var goodreadsBooks = new List<GoodreadsBook>
            {
                new GoodreadsBook
                {
                    Title = "Book Without ISBN",
                    AuthorName = "Test Author",
                    GoodreadsId = "111111",
                    Isbn = null, // Missing ISBN
                    PublicationYear = null // Missing year
                }
            };

            Mocker.GetMock<IGoodreadsProxy>()
                .Setup(s => s.GetListBooks(_settings.ListId, _settings.ApiKey))
                .Returns(goodreadsBooks);

            var result = Subject.Fetch();

            result.Books.Should().HaveCount(1);
            var book = result.Books.First();
            book.Isbn.Should().BeNull();
            book.Year.Should().Be(0);
        }

        [Test]
        public void should_record_success_on_successful_fetch()
        {
            Mocker.GetMock<IGoodreadsProxy>()
                .Setup(s => s.GetListBooks(_settings.ListId, _settings.ApiKey))
                .Returns(new List<GoodreadsBook>());

            Subject.Fetch();

            Mocker.GetMock<IImportListStatusService>()
                .Verify(v => v.RecordSuccess(1), Times.Once());
        }
    }
}