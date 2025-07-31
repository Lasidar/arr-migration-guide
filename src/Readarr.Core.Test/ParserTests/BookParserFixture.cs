using FluentAssertions;
using NUnit.Framework;
using Readarr.Core.Parser;
using Readarr.Core.Test.Framework;
using Readarr.Core.Tv;

namespace Readarr.Core.Test.ParserTests
{
    [TestFixture]
    public class BookParserFixture : CoreTest
    {
        [TestCase("Stephen King - The Stand (1978) EPUB", "Stephen King", "The Stand", 1978)]
        [TestCase("J.K. Rowling - Harry Potter and the Philosopher's Stone [EPUB]", "J.K. Rowling", "Harry Potter and the Philosopher's Stone", 0)]
        [TestCase("George R.R. Martin - A Game of Thrones (A Song of Ice and Fire #1) (1996)", "George R.R. Martin", "A Game of Thrones", 1996)]
        [TestCase("Brandon Sanderson - The Way of Kings (2010) [MOBI]", "Brandon Sanderson", "The Way of Kings", 2010)]
        [TestCase("Neil Gaiman - American Gods [2001]", "Neil Gaiman", "American Gods", 2001)]
        public void should_parse_author_title_year(string title, string expectedAuthor, string expectedTitle, int expectedYear)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.AuthorName.Should().Be(expectedAuthor);
            result.BookTitle.Should().Be(expectedTitle);
            if (expectedYear > 0)
            {
                result.ReleaseYear.Should().Be(expectedYear);
            }
        }

        [TestCase("The Stand by Stephen King (1978).epub", "Stephen King", "The Stand")]
        [TestCase("Harry Potter and the Philosopher's Stone by J.K. Rowling.mobi", "J.K. Rowling", "Harry Potter and the Philosopher's Stone")]
        [TestCase("A Game of Thrones by George R.R. Martin.azw3", "George R.R. Martin", "A Game of Thrones")]
        public void should_parse_title_by_author_format(string title, string expectedAuthor, string expectedTitle)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.AuthorName.Should().Be(expectedAuthor);
            result.BookTitle.Should().Be(expectedTitle);
        }

        [TestCase("Stephen_King-The_Stand-1978.EPUB", "Stephen King", "The Stand")]
        [TestCase("J.K.Rowling-Harry.Potter.and.the.Philosopher's.Stone.MOBI", "J.K.Rowling", "Harry Potter and the Philosopher's Stone")]
        [TestCase("George_R_R_Martin-A_Game_of_Thrones.AZW3", "George R R Martin", "A Game of Thrones")]
        public void should_parse_underscore_and_dot_separators(string title, string expectedAuthor, string expectedTitle)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.AuthorName.Should().Be(expectedAuthor);
            result.BookTitle.Should().Be(expectedTitle);
        }

        [TestCase("The Stand (Stephen King) [EPUB]", "Stephen King", "The Stand")]
        [TestCase("Harry Potter and the Philosopher's Stone (J.K. Rowling)", "J.K. Rowling", "Harry Potter and the Philosopher's Stone")]
        [TestCase("A Game of Thrones (George R.R. Martin) (1996)", "George R.R. Martin", "A Game of Thrones")]
        public void should_parse_title_with_author_in_parentheses(string title, string expectedAuthor, string expectedTitle)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.AuthorName.Should().Be(expectedAuthor);
            result.BookTitle.Should().Be(expectedTitle);
        }

        [TestCase("Stephen King - The Stand (Unabridged) (1978) EPUB", "The Stand", "Unabridged")]
        [TestCase("J.K. Rowling - Harry Potter (Illustrated Edition) [EPUB]", "Harry Potter", "Illustrated Edition")]
        [TestCase("Brandon Sanderson - The Way of Kings (10th Anniversary Edition) (2020)", "The Way of Kings", "10th Anniversary Edition")]
        public void should_parse_edition_info(string title, string expectedTitle, string expectedEdition)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.BookTitle.Should().Be(expectedTitle);
            result.ReleaseEdition.Should().Be(expectedEdition);
        }

        [TestCase("Stephen King - The Stand [Audiobook]", "Audiobook")]
        [TestCase("J.K. Rowling - Harry Potter [MP3]", "MP3")]
        [TestCase("George R.R. Martin - A Game of Thrones [M4B]", "M4B")]
        public void should_identify_audiobook_formats(string title, string expectedFormat)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.ReleaseFormat.Should().Be(expectedFormat);
        }

        [TestCase("978-0-385-12168-2", true)]
        [TestCase("0-385-12168-7", true)]
        [TestCase("978-0385121682", true)]
        [TestCase("0385121687", true)]
        [TestCase("not-an-isbn", false)]
        [TestCase("123456789", false)]
        public void should_identify_isbn(string input, bool expectedResult)
        {
            var result = Parser.IsValidIsbn(input);

            result.Should().Be(expectedResult);
        }

        [TestCase("Stephen King - The Dark Tower Series (Books 1-7) EPUB Collection", 7)]
        [TestCase("Harry Potter Complete Collection (Books 1-7) by J.K. Rowling", 7)]
        [TestCase("The Hunger Games Trilogy by Suzanne Collins", 3)]
        public void should_identify_book_collections(string title, int expectedCount)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.IsCollection.Should().BeTrue();
            result.CollectionBookCount.Should().Be(expectedCount);
        }

        [TestCase("Stephen King - The Stand (1978) RETAIL EPUB", true)]
        [TestCase("J.K. Rowling - Harry Potter [PROPER]", true)]
        [TestCase("George R.R. Martin - A Game of Thrones REPACK", true)]
        public void should_identify_special_releases(string title, bool expectedSpecial)
        {
            var result = Parser.ParseBookTitle(title);

            result.Should().NotBeNull();
            result.IsSpecialRelease.Should().Be(expectedSpecial);
        }
    }
}