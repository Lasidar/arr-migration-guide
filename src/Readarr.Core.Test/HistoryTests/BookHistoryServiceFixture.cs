using System;
using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Readarr.Core.Books;
using Readarr.Core.Download;
using Readarr.Core.History;
using Readarr.Core.MediaFiles;
using Readarr.Core.Parser.Model;
using Readarr.Core.Profiles.Qualities;
using Readarr.Core.Qualities;
using Readarr.Core.Test.Framework;

namespace Readarr.Core.Test.HistoryTests
{
    [TestFixture]
    public class BookHistoryServiceFixture : CoreTest<BookHistoryService>
    {
        private Author _author;
        private Book _book;
        private BookFile _bookFile;
        private QualityProfile _profile;

        [SetUp]
        public void Setup()
        {
            _profile = TestBuilders.CreateQualityProfile("Test Profile");
            _author = TestBuilders.CreateAuthor(1, "Test Author");
            _book = TestBuilders.CreateBook(1, "Test Book", _author);
            _bookFile = TestBuilders.CreateBookFile(1, _book, new QualityModel(Quality.EPUB));
        }

        [Test]
        public void should_get_download_history()
        {
            var historyItems = Builder<BookHistory>.CreateListOfSize(5)
                .All()
                .With(h => h.BookId = _book.Id)
                .With(h => h.EventType = BookHistoryEventType.Grabbed)
                .With(h => h.DownloadId = "test-download-id")
                .Build()
                .ToList();

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.FindDownloadHistory(_book.Id, It.IsAny<QualityModel>()))
                .Returns(historyItems);

            var result = Subject.GetByBook(_book.Id, BookHistoryEventType.Grabbed);

            result.Should().HaveCount(5);
            result.All(h => h.EventType == BookHistoryEventType.Grabbed).Should().BeTrue();
        }

        [Test]
        public void should_find_by_download_id()
        {
            var downloadId = "test-download-123";
            var historyItems = Builder<BookHistory>.CreateListOfSize(2)
                .All()
                .With(h => h.DownloadId = downloadId)
                .With(h => h.EventType = BookHistoryEventType.Grabbed)
                .Build()
                .ToList();

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.FindByDownloadId(downloadId))
                .Returns(historyItems);

            var result = Subject.FindByDownloadId(downloadId);

            result.Should().HaveCount(2);
            result.All(h => h.DownloadId == downloadId).Should().BeTrue();
        }

        [Test]
        public void should_mark_as_failed()
        {
            var downloadId = "test-download-123";
            var historyItems = Builder<BookHistory>.CreateListOfSize(2)
                .All()
                .With(h => h.DownloadId = downloadId)
                .With(h => h.EventType = BookHistoryEventType.Grabbed)
                .Build()
                .ToList();

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.FindByDownloadId(downloadId))
                .Returns(historyItems);

            Subject.MarkAsFailed(downloadId);

            Mocker.GetMock<IHistoryRepository>()
                .Verify(v => v.UpdateMany(It.Is<List<BookHistory>>(l => 
                    l.Count == 2 && 
                    l.All(h => h.EventType == BookHistoryEventType.DownloadFailed)
                )), Times.Once());
        }

        [Test]
        public void should_mark_as_imported()
        {
            var downloadId = "test-download-123";
            var historyItems = Builder<BookHistory>.CreateListOfSize(1)
                .All()
                .With(h => h.DownloadId = downloadId)
                .With(h => h.EventType = BookHistoryEventType.Grabbed)
                .Build()
                .ToList();

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.FindByDownloadId(downloadId))
                .Returns(historyItems);

            Subject.MarkAsImported(downloadId);

            Mocker.GetMock<IHistoryRepository>()
                .Verify(v => v.UpdateMany(It.Is<List<BookHistory>>(l => 
                    l.Count == 1 && 
                    l.All(h => h.EventType == BookHistoryEventType.BookFileImported)
                )), Times.Once());
        }

        [Test]
        public void should_get_most_recent_for_book()
        {
            var bookId = 123;
            var mostRecent = new BookHistory
            {
                BookId = bookId,
                EventType = BookHistoryEventType.BookFileImported,
                Date = DateTime.UtcNow
            };

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.MostRecentForBook(bookId))
                .Returns(mostRecent);

            var result = Subject.MostRecentForBook(bookId);

            result.Should().NotBeNull();
            result.BookId.Should().Be(bookId);
            result.EventType.Should().Be(BookHistoryEventType.BookFileImported);
        }

        [Test]
        public void should_get_most_recent_for_download_id()
        {
            var downloadId = "test-download-123";
            var mostRecent = new BookHistory
            {
                DownloadId = downloadId,
                EventType = BookHistoryEventType.Grabbed,
                Date = DateTime.UtcNow
            };

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.MostRecentForDownloadId(downloadId))
                .Returns(mostRecent);

            var result = Subject.MostRecentForDownloadId(downloadId);

            result.Should().NotBeNull();
            result.DownloadId.Should().Be(downloadId);
        }

        [Test]
        public void should_find_by_source_title()
        {
            var sourceTitle = "Test.Book.EPUB-GROUP";
            var historyItems = Builder<BookHistory>.CreateListOfSize(3)
                .All()
                .With(h => h.SourceTitle = sourceTitle)
                .Build()
                .ToList();

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.FindBySourceTitle(sourceTitle))
                .Returns(historyItems);

            var result = Subject.FindBySourceTitle(sourceTitle);

            result.Should().HaveCount(3);
            result.All(h => h.SourceTitle == sourceTitle).Should().BeTrue();
        }

        [Test]
        public void should_get_by_author()
        {
            var authorId = 123;
            var eventType = BookHistoryEventType.Grabbed;
            var historyItems = Builder<BookHistory>.CreateListOfSize(4)
                .All()
                .With(h => h.AuthorId = authorId)
                .With(h => h.EventType = eventType)
                .Build()
                .ToList();

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.GetByAuthor(authorId, eventType))
                .Returns(historyItems);

            var result = Subject.GetByAuthor(authorId, eventType);

            result.Should().HaveCount(4);
            result.All(h => h.AuthorId == authorId && h.EventType == eventType).Should().BeTrue();
        }

        [Test]
        public void should_purge_old_history()
        {
            Subject.Cleanup();

            Mocker.GetMock<IHistoryRepository>()
                .Verify(v => v.Cleanup(30), Times.Once());
        }

        [Test]
        public void should_get_since_date()
        {
            var since = DateTime.UtcNow.AddDays(-7);
            var eventType = BookHistoryEventType.BookFileImported;
            var historyItems = Builder<BookHistory>.CreateListOfSize(3)
                .All()
                .With(h => h.EventType = eventType)
                .With(h => h.Date = DateTime.UtcNow.AddDays(-3))
                .Build()
                .ToList();

            Mocker.GetMock<IHistoryRepository>()
                .Setup(s => s.Since(since, eventType))
                .Returns(historyItems);

            var result = Subject.Since(since, eventType);

            result.Should().HaveCount(3);
            result.All(h => h.Date > since && h.EventType == eventType).Should().BeTrue();
        }
    }
}