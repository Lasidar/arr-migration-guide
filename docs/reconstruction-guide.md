# Reconstruction Guide: Building Readarr from Sonarr

## Overview
This guide provides step-by-step instructions for recreating Readarr starting from Sonarr's codebase. It documents the complete transformation path and serves as a blueprint for future media type adaptations.

## Prerequisites

- Sonarr source code at commit `83370dd` (or latest stable)
- .NET development environment
- Git for version control
- Understanding of the domain models for both TV shows and books

## Phase 1: Initial Fork and Setup (Day 1-2)

### Step 1.1: Create Fork
```bash
# Fork Sonarr repository
git clone https://github.com/Sonarr/Sonarr.git readarr
cd readarr
git remote rename origin sonarr-upstream
git remote add origin https://github.com/Readarr/Readarr.git
```

### Step 1.2: Initial Cleanup
```bash
# Create new branch for transformation
git checkout -b initial-readarr-transformation

# Update project names
find . -name "*.csproj" -exec sed -i 's/Sonarr/Readarr/g' {} \;
find . -name "*.cs" -exec sed -i 's/Sonarr/Readarr/g' {} \;
```

### Step 1.3: Update Branding
- Replace all UI branding elements
- Update application name in configuration
- Change default ports (8989 → 8787)
- Update user agent strings

**Files to modify:**
- `src/NzbDrone.Core/Configuration/ConfigService.cs`
- `src/NzbDrone.Host/ApplicationServer.cs`
- `frontend/src/index.html`
- `package.json`

## Phase 2: Domain Model Transformation (Day 3-7)

### Step 2.1: Create Author Model
Transform Series → Author:

```csharp
// src/NzbDrone.Core/Books/Author.cs
namespace NzbDrone.Core.Books
{
    public class Author : Entity<Author>
    {
        public int AuthorMetadataId { get; set; }
        public string CleanName { get; set; }
        public bool Monitored { get; set; }
        public DateTime? LastInfoSync { get; set; }
        public string Path { get; set; }
        public string RootFolderPath { get; set; }
        public DateTime Added { get; set; }
        public int QualityProfileId { get; set; }
        public int MetadataProfileId { get; set; }
        public HashSet<int> Tags { get; set; }
        
        // Navigation properties
        public LazyLoaded<AuthorMetadata> Metadata { get; set; }
        public LazyLoaded<List<Book>> Books { get; set; }
        public LazyLoaded<List<Series>> Series { get; set; }
    }
}
```

### Step 2.2: Create AuthorMetadata Model
New pattern introduced for better data normalization:

```csharp
// src/NzbDrone.Core/Books/AuthorMetadata.cs
namespace NzbDrone.Core.Books
{
    public class AuthorMetadata : Entity<AuthorMetadata>
    {
        public string ForeignAuthorId { get; set; }
        public string Name { get; set; }
        public string Overview { get; set; }
        public string Disambiguation { get; set; }
        public string Status { get; set; }
        public List<MediaCover> Images { get; set; }
        public List<Links> Links { get; set; }
        public List<string> Genres { get; set; }
        public Ratings Ratings { get; set; }
    }
}
```

### Step 2.3: Create Book Model
Transform Season → Book:

```csharp
// src/NzbDrone.Core/Books/Book.cs
namespace NzbDrone.Core.Books
{
    public class Book : Entity<Book>
    {
        public int AuthorMetadataId { get; set; }
        public string ForeignBookId { get; set; }
        public string TitleSlug { get; set; }
        public string Title { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public List<Links> Links { get; set; }
        public List<string> Genres { get; set; }
        public List<int> RelatedBooks { get; set; }
        public Ratings Ratings { get; set; }
        
        // Readarr specific
        public bool Monitored { get; set; }
        public bool AnyEditionOk { get; set; }
        public DateTime? LastInfoSync { get; set; }
        public DateTime Added { get; set; }
        
        // Navigation properties
        public LazyLoaded<Author> Author { get; set; }
        public LazyLoaded<List<Edition>> Editions { get; set; }
        public LazyLoaded<List<BookFile>> BookFiles { get; set; }
    }
}
```

### Step 2.4: Create Edition Model
Transform Episode → Edition:

```csharp
// src/NzbDrone.Core/Books/Edition.cs
namespace NzbDrone.Core.Books
{
    public class Edition : Entity<Edition>
    {
        public int BookId { get; set; }
        public string ForeignEditionId { get; set; }
        public string TitleSlug { get; set; }
        public string Title { get; set; }
        public string ISBN13 { get; set; }
        public string ASIN { get; set; }
        public bool Monitored { get; set; }
        public int PageCount { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Format { get; set; }
        
        // Navigation properties
        public LazyLoaded<Book> Book { get; set; }
        public LazyLoaded<List<BookFile>> BookFiles { get; set; }
    }
}
```

### Step 2.5: Update Database Migrations
Create initial migration for book schema:

```csharp
// src/NzbDrone.Core/Datastore/Migration/001_initial_readarr.cs
public class initial_readarr : NzbDroneMigrationBase
{
    protected override void MainDbUpgrade()
    {
        // Create AuthorMetadata table
        Create.Table("AuthorMetadata")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ForeignAuthorId").AsString().Unique()
            .WithColumn("Name").AsString()
            .WithColumn("Overview").AsString().Nullable()
            // ... additional columns
            
        // Create Authors table
        Create.Table("Authors")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("AuthorMetadataId").AsInt32()
            .WithColumn("CleanName").AsString()
            // ... additional columns
            
        // Create Books table
        Create.Table("Books")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("AuthorMetadataId").AsInt32()
            .WithColumn("ForeignBookId").AsString()
            // ... additional columns
            
        // Create Editions table
        Create.Table("Editions")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("BookId").AsInt32()
            .WithColumn("ForeignEditionId").AsString()
            // ... additional columns
    }
}
```

## Phase 3: Service Layer Transformation (Day 8-12)

### Step 3.1: Create Author Service
Transform SeriesService → AuthorService:

```csharp
// src/NzbDrone.Core/Books/Services/AuthorService.cs
public interface IAuthorService
{
    Author GetAuthor(int authorId);
    List<Author> GetAuthors(IEnumerable<int> authorIds);
    Author AddAuthor(Author newAuthor);
    List<Author> AddAuthors(List<Author> newAuthors);
    Author FindByName(string cleanName);
    Author FindById(string foreignId);
    void DeleteAuthor(int authorId, bool deleteFiles);
    List<Author> GetAllAuthors();
    List<Author> AllForTag(int tagId);
    Author UpdateAuthor(Author author);
    List<Author> UpdateAuthors(List<Author> authors);
    bool AuthorPathExists(string folder);
    void RemoveAddOptions(Author author);
}

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IEventAggregator _eventAggregator;
    private readonly IAuthorMetadataService _authorMetadataService;
    private readonly IBuildFileNames _fileNameBuilder;
    
    // Implementation following Sonarr's SeriesService pattern
}
```

### Step 3.2: Create Metadata Services
New pattern for handling external metadata:

```csharp
// src/NzbDrone.Core/Books/Services/AuthorMetadataService.cs
public interface IAuthorMetadataService
{
    AuthorMetadata Get(int id);
    AuthorMetadata FindById(string foreignId);
    AuthorMetadata Upsert(AuthorMetadata author);
}
```

### Step 3.3: Transform Refresh Services
RefreshSeriesService → RefreshAuthorService:

```csharp
// src/NzbDrone.Core/Books/Services/RefreshAuthorService.cs
public class RefreshAuthorService : IExecute<RefreshAuthorCommand>
{
    private readonly IProvideAuthorInfo _authorInfo;
    private readonly IAuthorService _authorService;
    private readonly IAuthorMetadataService _authorMetadataService;
    private readonly IBookService _bookService;
    private readonly IEventAggregator _eventAggregator;
    
    public void Execute(RefreshAuthorCommand message)
    {
        // Refresh logic adapted for books
    }
}
```

## Phase 4: Metadata Provider Integration (Day 13-16)

### Step 4.1: Define Provider Interfaces
```csharp
// src/NzbDrone.Core/MetadataSource/IProvideAuthorInfo.cs
public interface IProvideAuthorInfo
{
    Author GetAuthorInfo(string foreignAuthorId);
    List<Author> SearchForNewAuthor(string title);
}

// src/NzbDrone.Core/MetadataSource/IProvideBookInfo.cs
public interface IProvideBookInfo
{
    Book GetBookInfo(string foreignBookId);
    List<Book> GetBooksForAuthor(int authorMetadataId);
}
```

### Step 4.2: Implement Goodreads Provider
Replace TheTVDB with Goodreads:

```csharp
// src/NzbDrone.Core/MetadataSource/BookInfo/BookInfoProxy.cs
public class BookInfoProxy : IProvideAuthorInfo, IProvideBookInfo
{
    private readonly IHttpClient _httpClient;
    private readonly IAuthorService _authorService;
    private readonly IBookService _bookService;
    
    public Author GetAuthorInfo(string foreignAuthorId)
    {
        // Call Goodreads API
        // Map response to Author model
        // Return enriched Author
    }
}
```

### Step 4.3: Update Search Indexers
Modify indexer search parameters:

```csharp
// src/NzbDrone.Core/Indexers/Newznab/NewznabRequestGenerator.cs
// Add book-specific search parameters
private void AddBookSearchParameters(SearchCriteriaBase searchCriteria)
{
    if (searchCriteria is BookSearchCriteria bookCriteria)
    {
        queryParams.Add("author", bookCriteria.AuthorName);
        queryParams.Add("title", bookCriteria.BookTitle);
        queryParams.Add("isbn", bookCriteria.ISBN);
    }
}
```

## Phase 5: API Layer Transformation (Day 17-19)

### Step 5.1: Create Author Controller
Transform SeriesController → AuthorController:

```csharp
// src/Readarr.Api.V1/Author/AuthorController.cs
[V1ApiController]
public class AuthorController : RestControllerWithSignalR<AuthorResource, Author>
{
    private readonly IAuthorService _authorService;
    private readonly IAddAuthorService _addAuthorService;
    private readonly IAuthorStatisticsService _authorStatisticsService;
    
    [HttpGet]
    public List<AuthorResource> GetAuthors()
    {
        var authors = _authorService.GetAllAuthors();
        return authors.ToResource();
    }
    
    [HttpPost]
    public ActionResult<AuthorResource> AddAuthor(AuthorResource authorResource)
    {
        var author = _addAuthorService.AddAuthor(authorResource.ToModel());
        return Created(author.Id);
    }
}
```

### Step 5.2: Create Book Controller
```csharp
// src/Readarr.Api.V1/Books/BookController.cs
[V1ApiController]
public class BookController : RestControllerWithSignalR<BookResource, Book>
{
    // Similar pattern to AlbumController in Lidarr
}
```

### Step 5.3: Update API Resources
Create resource models for API:

```csharp
// src/Readarr.Api.V1/Author/AuthorResource.cs
public class AuthorResource : RestResource
{
    public string AuthorName { get; set; }
    public string ForeignAuthorId { get; set; }
    public string Overview { get; set; }
    public List<BookResource> Books { get; set; }
    // ... additional properties
}
```

## Phase 6: UI Transformation (Day 20-22)

### Step 6.1: Update Frontend Routes
```typescript
// frontend/src/App/AppRoutes.tsx
const routes = [
  { path: '/authors', component: AuthorIndex },
  { path: '/author/:titleSlug', component: AuthorDetails },
  { path: '/add/search', component: AddNewAuthor },
  { path: '/bookshelf', component: Bookshelf },
  // ... additional routes
];
```

### Step 6.2: Create Author Components
Transform Series components:

```typescript
// frontend/src/Author/Index/AuthorIndex.tsx
const AuthorIndex: React.FC = () => {
  const authors = useSelector(getAllAuthors);
  
  return (
    <PageContent title="Authors">
      <AuthorIndexTable authors={authors} />
    </PageContent>
  );
};
```

### Step 6.3: Update Terminology
Global find/replace for UI strings:
- "Series" → "Author"
- "Season" → "Book" 
- "Episode" → "Edition"
- "TV Show" → "Book"
- "Air Date" → "Release Date"

## Phase 7: Quality and Format Handling (Day 23-24)

### Step 7.1: Define Book Qualities
```csharp
// src/NzbDrone.Core/Qualities/Quality.cs
public static class Quality
{
    public static Quality EPUB => new Quality(1, "EPUB", QualitySource.Ebook);
    public static Quality MOBI => new Quality(2, "MOBI", QualitySource.Ebook);
    public static Quality AZW3 => new Quality(3, "AZW3", QualitySource.Ebook);
    public static Quality PDF => new Quality(4, "PDF", QualitySource.Ebook);
    public static Quality MP3 => new Quality(10, "MP3", QualitySource.Audiobook);
    public static Quality M4B => new Quality(11, "M4B", QualitySource.Audiobook);
}
```

### Step 7.2: Create Metadata Profiles
New feature for book preferences:

```csharp
// src/NzbDrone.Core/Profiles/Metadata/MetadataProfile.cs
public class MetadataProfile : ModelBase
{
    public string Name { get; set; }
    public List<MetadataProfileItem> AllowedFormats { get; set; }
    public int MinPages { get; set; }
    public bool SkipAbridged { get; set; }
}
```

## Phase 8: Testing and Validation (Day 25-27)

### Step 8.1: Update Unit Tests
Transform all test cases:

```csharp
// src/NzbDrone.Core.Test/BookTests/AuthorServiceTests.cs
[TestFixture]
public class AuthorServiceTests : CoreTest<AuthorService>
{
    [Test]
    public void should_add_author()
    {
        // Test implementation
    }
}
```

### Step 8.2: Integration Tests
Create book-specific integration tests:

```csharp
// src/NzbDrone.Integration.Test/ApiTests/AuthorFixture.cs
[TestFixture]
public class AuthorFixture : IntegrationTest
{
    [Test]
    public void should_add_author_from_goodreads()
    {
        // Test Goodreads integration
    }
}
```

### Step 8.3: Validation Checklist
- [ ] Authors can be added from search
- [ ] Books are properly associated with authors
- [ ] Editions can be monitored independently
- [ ] Quality profiles work correctly
- [ ] File naming follows patterns
- [ ] Import process handles book files
- [ ] Metadata refresh works
- [ ] Download clients integrate properly

## Phase 9: Final Polish (Day 28-30)

### Step 9.1: Performance Optimization
- Implement lazy loading for author/book relationships
- Add database indexes for common queries
- Optimize metadata refresh operations

### Step 9.2: Migration from Calibre
Create import functionality:

```csharp
// src/NzbDrone.Core/Books/Calibre/CalibreImporter.cs
public class CalibreImporter : IExecute<ImportFromCalibreCommand>
{
    public void Execute(ImportFromCalibreCommand command)
    {
        // Import logic from Calibre database
    }
}
```

### Step 9.3: Documentation
- Update API documentation
- Create user migration guide
- Document new features

## Troubleshooting Guide

### Common Issues and Solutions

**Issue**: Database migrations fail
**Solution**: Ensure all foreign key relationships are properly defined

**Issue**: Metadata provider timeouts
**Solution**: Implement retry logic and rate limiting

**Issue**: Import failures for certain formats
**Solution**: Add format-specific parsers

**Issue**: UI components not updating
**Solution**: Check SignalR connections and event publishing

## Validation Checkpoints

After each phase, validate:

1. **Phase 1**: Application starts and shows rebranded UI
2. **Phase 2**: Database schema is created correctly
3. **Phase 3**: Authors can be added manually
4. **Phase 4**: Metadata providers return data
5. **Phase 5**: API endpoints respond correctly
6. **Phase 6**: UI displays author/book information
7. **Phase 7**: Quality profiles can be configured
8. **Phase 8**: All tests pass
9. **Phase 9**: Full workflow works end-to-end

## Conclusion

This guide provides the complete path from Sonarr to Readarr. Key transformations include:

1. Domain model shift from TV to books
2. Introduction of metadata separation pattern
3. Addition of edition concept for multiple book formats
4. Integration with book-specific metadata providers
5. UI adaptation for book management

Following this guide, a developer can recreate Readarr's functionality starting from Sonarr's codebase, understanding not just what changed but why and how to implement similar transformations for other media types.