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

namespace Readarr.Core.Test.AuthorTests.AuthorServiceTests
{
    [TestFixture]
    public class AuthorServiceFixture : CoreTest<AuthorService>
    {
        private List<Author> _authors;

        [SetUp]
        public void Setup()
        {
            _authors = new List<Author>
            {
                TestBuilders.CreateAuthor(1, "Author One"),
                TestBuilders.CreateAuthor(2, "Author Two"),
                TestBuilders.CreateAuthor(3, "Author Three")
            };

            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.All())
                .Returns(_authors);
        }

        [Test]
        public void should_get_all_authors()
        {
            var result = Subject.GetAllAuthors();

            result.Should().HaveCount(3);
            result.Should().BeEquivalentTo(_authors);
        }

        [Test]
        public void should_find_author_by_id()
        {
            var author = _authors.First();
            
            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.Get(author.Id))
                .Returns(author);

            var result = Subject.GetAuthor(author.Id);

            result.Should().NotBeNull();
            result.Id.Should().Be(author.Id);
            result.Name.Should().Be(author.Name);
        }

        [Test]
        public void should_throw_when_author_not_found()
        {
            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.Get(999))
                .Returns((Author)null);

            Assert.Throws<AuthorNotFoundException>(() => Subject.GetAuthor(999));
        }

        [Test]
        public void should_add_author()
        {
            var newAuthor = TestBuilders.CreateAuthor(0, "New Author");
            newAuthor.Path = @"C:\Test\New Author";

            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.Insert(It.IsAny<Author>()))
                .Returns(newAuthor);

            Mocker.GetMock<ICheckIfAuthorShouldBeRefreshed>()
                .Setup(s => s.ShouldRefresh(It.IsAny<Author>()))
                .Returns(false);

            var result = Subject.AddAuthor(newAuthor);

            result.Should().NotBeNull();
            
            Mocker.GetMock<IAuthorRepository>()
                .Verify(v => v.Insert(newAuthor), Times.Once());

            Mocker.GetMock<IEventAggregator>()
                .Verify(v => v.PublishEvent(It.IsAny<AuthorAddedEvent>()), Times.Once());
        }

        [Test]
        public void should_update_author()
        {
            var author = _authors.First();
            author.Name = "Updated Author";

            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.Update(author))
                .Returns(author);

            var result = Subject.UpdateAuthor(author);

            result.Name.Should().Be("Updated Author");

            Mocker.GetMock<IAuthorRepository>()
                .Verify(v => v.Update(author), Times.Once());

            Mocker.GetMock<IEventAggregator>()
                .Verify(v => v.PublishEvent(It.IsAny<AuthorEditedEvent>()), Times.Once());
        }

        [Test]
        public void should_delete_author()
        {
            var author = _authors.First();

            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.Get(author.Id))
                .Returns(author);

            Subject.DeleteAuthor(author.Id, true, false);

            Mocker.GetMock<IAuthorRepository>()
                .Verify(v => v.Delete(author.Id), Times.Once());

            Mocker.GetMock<IEventAggregator>()
                .Verify(v => v.PublishEvent(It.IsAny<AuthorDeletedEvent>()), Times.Once());
        }

        [Test]
        public void should_find_author_by_name()
        {
            var author = _authors.First();
            
            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.FindByName(author.CleanName))
                .Returns(author);

            var result = Subject.FindByName(author.Name);

            result.Should().NotBeNull();
            result.Name.Should().Be(author.Name);
        }

        [Test]
        public void should_get_authors_by_metadata_id()
        {
            var goodreadsId = "12345";
            var author = _authors.First();
            author.Metadata.Value.GoodreadsId = goodreadsId;

            Mocker.GetMock<IAuthorRepository>()
                .Setup(s => s.FindById(goodreadsId))
                .Returns(new List<Author> { author });

            var result = Subject.FindById(goodreadsId);

            result.Should().HaveCount(1);
            result.First().Metadata.Value.GoodreadsId.Should().Be(goodreadsId);
        }
    }
}