using System.Collections.Generic;
using System.Linq;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Readarr.Core.Books;
using Readarr.Core.Books.Events;
using Readarr.Core.Exceptions;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Test.Framework;
using Readarr.Test.Common;

namespace Readarr.Core.Test.BooksTests.BookServiceTests
{
    [TestFixture]
    public class BookServiceFixture : CoreTest<BookService>
    {
        private List<Book> _books;
        private Author _author;

        [SetUp]
        public void Setup()
        {
            _author = TestBuilders.CreateAuthor(1, "Test Author");
            
            _books = new List<Book>
            {
                TestBuilders.CreateBook(1, "Book 1", _author),
                TestBuilders.CreateBook(2, "Book 2", _author),
                TestBuilders.CreateBook(3, "Book 3", _author)
            };

            Mocker.GetMock<IBookRepository>()
                .Setup(s => s.GetBooksByAuthor(_author.Id))
                .Returns(_books);
        }

        [Test]
        public void should_get_all_books_by_author()
        {
            var result = Subject.GetBooksByAuthor(_author.Id);

            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_books);
        }

        [Test]
        public void should_find_book_by_id()
        {
            var book = _books.First();
            
            Mocker.GetMock<IBookRepository>()
                .Setup(s => s.Get(book.Id))
                .Returns(book);

            var result = Subject.GetBook(book.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(book.Id);
            result.Title.Should().Be(book.Title);
        }

        [Test]
        public void should_throw_when_book_not_found()
        {
            Mocker.GetMock<IBookRepository>()
                .Setup(s => s.Get(999))
                .Returns((Book)null);

            Assert.Throws<BookNotFoundException>(() => Subject.GetBook(999));
        }

        [Test]
        public void should_add_book()
        {
            var newBook = TestBuilders.CreateBook(0, "New Book", _author);

            Mocker.GetMock<IBookRepository>()
                .Setup(s => s.Insert(It.IsAny<Book>()))
                .Returns(newBook);

            var result = Subject.AddBook(newBook);

            result.Should().NotBeNull();
            
            Mocker.GetMock<IBookRepository>()
                .Verify(v => v.Insert(newBook), Times.Once());

            Mocker.GetMock<IEventAggregator>()
                .Verify(v => v.PublishEvent(It.IsAny<BookAddedEvent>()), Times.Once());
        }

        [Test]
        public void should_update_book()
        {
            var book = _books.First();
            book.Title = "Updated Title";

            Mocker.GetMock<IBookRepository>()
                .Setup(s => s.Update(book))
                .Returns(book);

            var result = Subject.UpdateBook(book);

            result.Title.Should().Be("Updated Title");

            Mocker.GetMock<IBookRepository>()
                .Verify(v => v.Update(book), Times.Once());

            Mocker.GetMock<IEventAggregator>()
                .Verify(v => v.PublishEvent(It.IsAny<BookEditedEvent>()), Times.Once());
        }

        [Test]
        public void should_delete_book()
        {
            var book = _books.First();

            Subject.DeleteBook(book.Id, true);

            Mocker.GetMock<IBookRepository>()
                .Verify(v => v.Delete(book.Id), Times.Once());

            Mocker.GetMock<IEventAggregator>()
                .Verify(v => v.PublishEvent(It.IsAny<BookDeletedEvent>()), Times.Once());
        }

        [Test]
        public void should_monitor_books()
        {
            var bookIds = _books.Select(b => b.Id).ToList();

            Subject.SetBookMonitored(bookIds, true);

            Mocker.GetMock<IBookRepository>()
                .Verify(v => v.SetMonitored(bookIds, true), Times.Once());
        }

        [Test]
        public void should_get_books_by_author_and_monitor_status()
        {
            var monitoredBooks = _books.Where(b => b.Id <= 2).ToList();

            Mocker.GetMock<IBookRepository>()
                .Setup(s => s.GetBooksByAuthor(_author.Id))
                .Returns(monitoredBooks);

            var result = Subject.GetBooksByAuthor(_author.Id, true);

            result.Should().HaveCount(2);
            result.Should().BeEquivalentTo(monitoredBooks);
        }
    }
}