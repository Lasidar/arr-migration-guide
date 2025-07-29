# Testing Patterns and Strategies

## Overview
This document analyzes the testing approaches used across Sonarr, Lidarr, and Readarr, identifying common patterns and evolution in testing strategies.

## Testing Framework Stack

### Common Testing Tools
- **Unit Testing**: NUnit 3.x
- **Mocking**: Moq
- **Assertions**: FluentAssertions
- **Integration Testing**: Custom framework
- **Database Testing**: In-memory SQLite
- **API Testing**: TestClient

## Test Organization Structure

### Project Structure
```
src/
├── NzbDrone.Core.Test/          # Core business logic tests
├── NzbDrone.Api.Test/           # API endpoint tests
├── NzbDrone.Common.Test/        # Common utilities tests
├── NzbDrone.Integration.Test/   # Full integration tests
├── NzbDrone.Automation.Test/    # UI automation tests
├── NzbDrone.Test.Common/        # Shared test utilities
└── NzbDrone.Libraries.Test/     # Third-party library tests
```

## Testing Pattern Categories

### 1. Base Test Class Pattern

**CoreTest<T> for Unit Tests:**
```csharp
public abstract class CoreTest<TSubject> : CoreTest where TSubject : class
{
    private TSubject _subject;

    [SetUp]
    public void CoreTestSetup()
    {
        _subject = null;
    }

    protected TSubject Subject
    {
        get
        {
            if (_subject == null)
            {
                _subject = Mocker.Resolve<TSubject>();
            }
            return _subject;
        }
    }
}
```

**DbTest for Database Tests:**
```csharp
public abstract class DbTest<TSubject, TModel> : DbTest
    where TSubject : class
    where TModel : ModelBase, new()
{
    protected BasicRepository<TModel> Storage { get; private set; }

    [SetUp]
    public void SetupReadDb()
    {
        Db = TestDatabase.Create();
        Storage = Mocker.Resolve<BasicRepository<TModel>>();
    }
}
```

### 2. Media-Specific Test Evolution

**Sonarr Series Tests:**
```csharp
[TestFixture]
public class SeriesServiceFixture : CoreTest<SeriesService>
{
    private Series _series;

    [SetUp]
    public void Setup()
    {
        _series = Builder<Series>.CreateNew()
            .With(s => s.Title = "Test Series")
            .With(s => s.TvdbId = 12345)
            .Build();
    }

    [Test]
    public void should_add_series()
    {
        // Arrange
        Mocker.GetMock<ISeriesRepository>()
            .Setup(s => s.Insert(It.IsAny<Series>()))
            .Returns(_series);

        // Act
        var result = Subject.AddSeries(_series);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Test Series");
    }
}
```

**Lidarr Artist Tests:**
```csharp
[TestFixture]
public class ArtistServiceFixture : CoreTest<ArtistService>
{
    private Artist _artist;
    private ArtistMetadata _metadata;

    [SetUp]
    public void Setup()
    {
        _metadata = Builder<ArtistMetadata>.CreateNew()
            .With(m => m.Name = "Test Artist")
            .Build();

        _artist = Builder<Artist>.CreateNew()
            .With(a => a.Metadata = new LazyLoaded<ArtistMetadata>(_metadata))
            .Build();
    }

    [Test]
    public void should_add_artist_with_metadata()
    {
        // Enhanced test with metadata handling
    }
}
```

**Readarr Author Tests:**
```csharp
[TestFixture]
public class AuthorServiceFixture : CoreTest<AuthorService>
{
    private Author _author;
    private List<Book> _books;

    [SetUp]
    public void Setup()
    {
        _author = Builder<Author>.CreateNew()
            .With(a => a.Name = "Test Author")
            .Build();

        _books = Builder<Book>.CreateListOfSize(3)
            .All()
            .With(b => b.AuthorMetadataId = _author.AuthorMetadataId)
            .Build()
            .ToList();
    }

    [Test]
    public void should_handle_author_with_series()
    {
        // Complex relationship testing
    }
}
```

### 3. Integration Test Patterns

**Base Integration Test:**
```csharp
public abstract class IntegrationTest
{
    protected RestClient RestClient { get; private set; }

    [SetUp]
    public void IntegrationSetup()
    {
        var factory = new WebApplicationFactory<Startup>();
        RestClient = new RestClient(factory.CreateClient());
    }

    [TearDown]
    public void IntegrationTearDown()
    {
        // Cleanup
    }
}
```

**Media-Specific Integration Tests:**
```csharp
[TestFixture]
public class AuthorIntegrationFixture : IntegrationTest
{
    [Test]
    public void should_add_author_from_search()
    {
        // Given
        var author = EnsureAuthor("J.K. Rowling", "1234");

        // When
        var result = Authors.Get(author.Id);

        // Then
        result.Should().NotBeNull();
        result.Books.Should().NotBeEmpty();
    }
}
```

### 4. Mocking Patterns

**Provider Mocking:**
```csharp
public class MetadataProviderTests
{
    [Test]
    public void should_get_author_info()
    {
        // Sonarr pattern
        Mocker.GetMock<IProvideSeriesInfo>()
            .Setup(s => s.GetSeriesInfo(It.IsAny<int>()))
            .Returns(new Series { Title = "Test" });

        // Readarr evolution
        Mocker.GetMock<IProvideAuthorInfo>()
            .Setup(s => s.GetAuthorInfo(It.IsAny<string>()))
            .Returns(new Author 
            { 
                Metadata = new AuthorMetadata { Name = "Test" }
            });
    }
}
```

### 5. Database Test Patterns

**Migration Testing:**
```csharp
[TestFixture]
public class MigrationFixture : MigrationTest<AddAuthorMetadata>
{
    [Test]
    public void should_migrate_author_data()
    {
        var db = WithMigrationTestDb(c =>
        {
            c.Insert.IntoTable("Authors").Row(new
            {
                Name = "Test Author",
                ForeignAuthorId = "1234"
            });
        });

        // Verify migration
        var authors = db.Query<Author>("SELECT * FROM Authors");
        authors.Should().HaveCount(1);
        authors.First().AuthorMetadataId.Should().BeGreaterThan(0);
    }
}
```

### 6. Quality Definition Testing

**Evolution of Quality Tests:**
```csharp
// Sonarr - Simple quality
[Test]
public void should_parse_hdtv_quality()
{
    var result = QualityParser.ParseQuality("Series.S01E01.HDTV.x264");
    result.Quality.Should().Be(Quality.HDTV720p);
}

// Lidarr - Format detection
[Test]
public void should_parse_flac_quality()
{
    var result = QualityParser.ParseQuality("Artist - Album [FLAC]");
    result.Quality.Should().Be(Quality.FLAC);
}

// Readarr - Multiple formats
[Test]
public void should_parse_epub_format()
{
    var result = QualityParser.ParseQuality("Author - Book.epub");
    result.Quality.Should().Be(Quality.EPUB);
}
```

### 7. Search and Indexer Testing

**Search Criteria Testing:**
```csharp
[TestFixture]
public class SearchCriteriaFixture
{
    [Test]
    public void should_create_book_search_criteria()
    {
        var criteria = new BookSearchCriteria
        {
            Author = "Stephen King",
            Title = "The Stand",
            ISBN = "978-0307743688"
        };

        criteria.GetQueryTitle().Should().Contain("Stephen King");
        criteria.GetQueryTitle().Should().Contain("The Stand");
    }
}
```

### 8. File Import Testing

**Import Decision Testing:**
```csharp
[TestFixture]
public class ImportDecisionMakerFixture : CoreTest<ImportDecisionMaker>
{
    [Test]
    public void should_accept_valid_book_file()
    {
        var localBook = new LocalBook
        {
            Path = @"C:\Books\Author - Title.epub",
            Quality = new QualityModel(Quality.EPUB),
            Book = Builder<Book>.CreateNew().Build()
        };

        var decisions = Subject.GetImportDecisions(new[] { localBook });
        
        decisions.Should().HaveCount(1);
        decisions.First().Accepted.Should().BeTrue();
    }
}
```

### 9. Performance Testing Patterns

**Large Dataset Testing:**
```csharp
[TestFixture]
[Category("Performance")]
public class LargeLibraryPerformanceFixture : DbTest
{
    [Test]
    [Timeout(5000)]
    public void should_handle_large_author_library()
    {
        // Insert 10,000 authors
        var authors = Builder<Author>.CreateListOfSize(10000).Build();
        Db.InsertMany(authors);

        // Performance assertion
        var sw = Stopwatch.StartNew();
        var result = Subject.GetAllAuthors();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(1000);
    }
}
```

### 10. UI Automation Testing

**Selenium-based Tests:**
```csharp
[TestFixture]
[Category("Automation")]
public class AuthorPageAutomationFixture : AutomationTest
{
    [Test]
    public void should_add_author_via_ui()
    {
        NavigateTo("/add/search");
        
        SearchBox.SendKeys("Brandon Sanderson");
        SearchButton.Click();
        
        WaitForElement(By.ClassName("search-result"));
        
        FirstSearchResult.Click();
        AddButton.Click();
        
        WaitForNavigation("/author/brandon-sanderson");
        
        PageTitle.Should().Contain("Brandon Sanderson");
    }
}
```

## Test Data Patterns

### 1. Builder Pattern Usage
```csharp
public class TestBuilders
{
    public static Author CreateAuthor()
    {
        return Builder<Author>.CreateNew()
            .With(a => a.Id = 0)
            .With(a => a.Name = "Test Author")
            .With(a => a.CleanName = "testauthor")
            .With(a => a.Monitored = true)
            .Build();
    }
}
```

### 2. Fixture Data
```csharp
public static class TestFixture
{
    public static readonly Quality[] AllQualities = 
    {
        Quality.EPUB,
        Quality.MOBI,
        Quality.AZW3,
        Quality.PDF
    };
}
```

## Testing Best Practices Observed

### 1. Arrange-Act-Assert Pattern
All tests follow AAA pattern with clear separation.

### 2. Test Isolation
Each test is independent with proper setup/teardown.

### 3. Meaningful Test Names
Tests use descriptive names following `should_[expected_behavior]_when_[condition]` pattern.

### 4. Category Attribution
Tests are categorized for selective execution:
- `[Category("Integration")]`
- `[Category("Performance")]`
- `[Category("LongRunning")]`

### 5. Timeout Management
Long-running tests have explicit timeouts:
```csharp
[Test]
[Timeout(30000)]
public void should_complete_within_timeout()
{
    // Test implementation
}
```

## Evolution of Testing Approaches

### Sonarr (Original)
- Basic unit tests
- Simple mocking
- Limited integration tests

### Lidarr (Enhanced)
- Better test organization
- More comprehensive mocking
- Improved integration test coverage

### Readarr (Mature)
- Full test pyramid implementation
- Performance test suite
- Comprehensive integration coverage
- Better test data management

## Testing Metrics

### Coverage Goals
- Unit Tests: >80% code coverage
- Integration Tests: All critical paths
- UI Tests: Key user workflows

### Test Execution
- Unit Tests: < 1 minute
- Integration Tests: < 5 minutes
- Full Suite: < 15 minutes

## Future Testing Improvements

### 1. Contract Testing
Implement consumer-driven contract tests for API compatibility.

### 2. Property-Based Testing
Use FsCheck for property-based testing of parsers and algorithms.

### 3. Mutation Testing
Implement mutation testing to verify test quality.

### 4. Load Testing
Add comprehensive load testing for large libraries.

## Conclusion

The testing patterns show:

1. **Consistent Framework**: Shared testing infrastructure across projects
2. **Progressive Enhancement**: Each project improves test coverage and patterns
3. **Media-Specific Adaptations**: Tests adapted for domain requirements
4. **Comprehensive Coverage**: Full test pyramid implementation
5. **Performance Focus**: Explicit performance testing for scalability

These patterns ensure high quality and maintainability across the *arr ecosystem.