# Readarr v2 Developer Guide

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 / VS Code / Rider
- SQLite (default) or PostgreSQL
- Git

### Setting Up Development Environment

1. **Clone the Repository**
```bash
git clone https://github.com/Readarr/Readarr.git
cd Readarr
git checkout readarrv2-dev2
```

2. **Restore Dependencies**
```bash
dotnet restore src/Readarr.sln
```

3. **Build the Solution**
```bash
dotnet build src/Readarr.sln
```

4. **Run the Application**
```bash
cd src/Readarr.Host
dotnet run
```

The application will be available at `http://localhost:7878`

## Architecture Overview

### Project Structure
```
src/
├── Readarr.Host/              # Application entry point
├── Readarr.Core/              # Core business logic
│   ├── Books/                 # Book domain models and services
│   ├── MediaFiles/            # File management
│   ├── Download/              # Download handling
│   ├── Indexers/              # Indexer integration
│   └── MetadataSource/        # External metadata
├── Readarr.Api.V1/            # REST API v1
├── Readarr.Api.V3/            # REST API v3
├── Readarr.Http/              # HTTP infrastructure
└── Readarr.Common/            # Shared utilities
```

### Key Design Patterns

#### Repository Pattern
```csharp
public interface IAuthorRepository : IBasicRepository<Author>
{
    Author FindByName(string name);
    List<Author> GetAuthorsWithPath(string path);
}

public class AuthorRepository : BasicRepository<Author>, IAuthorRepository
{
    public AuthorRepository(IMainDatabase database) : base(database)
    {
    }
    
    // Implementation
}
```

#### Service Layer
```csharp
public interface IAuthorService
{
    Author GetAuthor(int id);
    Author AddAuthor(Author author);
    void UpdateAuthor(Author author);
    void DeleteAuthor(int id);
}

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IEventAggregator _eventAggregator;
    
    // Implementation
}
```

#### Command Pattern for Jobs
```csharp
public class RefreshAuthorCommand : Command
{
    public int? AuthorId { get; set; }
}

public class RefreshAuthorService : IExecute<RefreshAuthorCommand>
{
    public void Execute(RefreshAuthorCommand command)
    {
        // Implementation
    }
}
```

## Adding New Features

### 1. Adding a New Domain Model

**Step 1**: Create the model in `Core/Books/`
```csharp
public class BookNote : ModelBase
{
    public int BookId { get; set; }
    public string Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Step 2**: Create repository interface and implementation
```csharp
public interface IBookNoteRepository : IBasicRepository<BookNote>
{
    List<BookNote> GetNotesForBook(int bookId);
}
```

**Step 3**: Add to TableMapping
```csharp
Mapper.Entity<BookNote>("BookNotes").RegisterModel();
```

**Step 4**: Create service
```csharp
public interface IBookNoteService
{
    BookNote AddNote(int bookId, string note);
    List<BookNote> GetNotes(int bookId);
}
```

### 2. Adding a New API Endpoint

**Step 1**: Create resource model
```csharp
public class BookNoteResource : RestResource
{
    public int BookId { get; set; }
    public string Note { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

**Step 2**: Create controller
```csharp
[V3ApiController]
public class BookNoteController : RestController<BookNoteResource>
{
    private readonly IBookNoteService _noteService;
    
    [HttpGet]
    public List<BookNoteResource> GetNotes(int bookId)
    {
        return _noteService.GetNotes(bookId).ToResource();
    }
    
    [HttpPost]
    public ActionResult<BookNoteResource> AddNote(BookNoteResource resource)
    {
        var note = _noteService.AddNote(resource.BookId, resource.Note);
        return Created(note.Id);
    }
}
```

### 3. Adding a Background Job

**Step 1**: Create command
```csharp
public class UpdateBookCoversCommand : Command
{
    public int? AuthorId { get; set; }
}
```

**Step 2**: Create service
```csharp
public class UpdateBookCoversService : IExecute<UpdateBookCoversCommand>
{
    public void Execute(UpdateBookCoversCommand command)
    {
        // Implementation
    }
}
```

**Step 3**: Schedule the job
```csharp
_taskManager.AddTask(typeof(UpdateBookCoversCommand), "Update Book Covers", 24 * 60);
```

## Working with the Database

### Migrations
Create a new migration:
```csharp
public class AddBookNotes : NzbDroneMigrationBase
{
    protected override void MainDbUpgrade()
    {
        Create.TableForModel("BookNotes")
            .WithColumn("BookId").AsInt32().NotNullable()
            .WithColumn("Note").AsString().NotNullable()
            .WithColumn("CreatedAt").AsDateTime().NotNullable();
    }
}
```

### Direct Database Access
```csharp
using (var mapper = _database.GetDataMapper())
{
    var books = mapper.Query<Book>(@"
        SELECT * FROM Books 
        WHERE AuthorId = @authorId 
        ORDER BY ReleaseDate DESC", 
        new { authorId });
}
```

## Testing

### Unit Tests
```csharp
[TestFixture]
public class AuthorServiceFixture : CoreTest<AuthorService>
{
    [Test]
    public void should_add_author()
    {
        var author = new Author { Name = "Test Author" };
        
        Subject.AddAuthor(author);
        
        Mocker.GetMock<IAuthorRepository>()
            .Verify(v => v.Insert(author), Times.Once());
    }
}
```

### Integration Tests
```csharp
[TestFixture]
public class AuthorControllerFixture : IntegrationTest
{
    [Test]
    public void should_get_all_authors()
    {
        var response = RestClient.Get<List<AuthorResource>>("/api/v1/author");
        
        response.Should().NotBeNull();
        response.Should().BeEmpty();
    }
}
```

## Common Tasks

### Adding a New Book Format
1. Update `BookExtensions` in `DiskScanService`
2. Add quality definition if needed
3. Update parser to recognize format
4. Add to import specifications

### Adding a Metadata Provider
1. Implement `IProvideAuthorInfo` or `IProvideBookInfo`
2. Add provider settings
3. Register in dependency injection
4. Add to metadata refresh service

### Adding an Indexer
1. Create indexer implementation
2. Add settings model
3. Create request generator
4. Implement response parser

## Debugging

### Enable Debug Logging
```csharp
LogManager.Configuration.LoggingRules.Add(
    new LoggingRule("*", LogLevel.Debug, consoleTarget));
```

### Common Issues

**Issue**: Dependency injection errors
**Solution**: Check registration in `MainModule.cs`

**Issue**: Database errors
**Solution**: Check migrations and table mappings

**Issue**: API 404 errors
**Solution**: Verify controller has correct attributes

## Performance Tips

### Database Queries
- Use eager loading for related data
- Avoid N+1 queries
- Use pagination for large datasets
- Index frequently queried columns

### API Responses
- Use projection to limit data
- Implement caching where appropriate
- Use async/await properly
- Minimize database calls

### Background Jobs
- Use bulk operations
- Implement proper cancellation
- Add progress reporting
- Handle failures gracefully

## Code Style Guidelines

### Naming Conventions
- Interfaces: `IAuthorService`
- Implementations: `AuthorService`
- Models: `Author`, `Book`
- Resources: `AuthorResource`
- Commands: `RefreshAuthorCommand`

### Best Practices
1. Use dependency injection
2. Keep methods small and focused
3. Handle exceptions appropriately
4. Log important operations
5. Write self-documenting code
6. Add XML documentation for public APIs

### Example Service Implementation
```csharp
public class BookRecommendationService : IBookRecommendationService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorService _authorService;
    private readonly ILogger _logger;
    
    public BookRecommendationService(
        IBookRepository bookRepository,
        IAuthorService authorService,
        Logger logger)
    {
        _bookRepository = bookRepository;
        _authorService = authorService;
        _logger = logger;
    }
    
    public List<Book> GetRecommendations(int authorId, int count = 10)
    {
        _logger.Debug("Getting recommendations for author {0}", authorId);
        
        try
        {
            var author = _authorService.GetAuthor(authorId);
            var books = _bookRepository.GetBooksByAuthor(authorId);
            
            // Implementation logic
            
            return recommendations;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to get recommendations");
            throw;
        }
    }
}
```

## Deployment

### Building for Production
```bash
dotnet publish src/Readarr.Host/Readarr.Host.csproj \
    -c Release \
    -r win-x64 \
    --self-contained false \
    -o ./publish
```

### Configuration
- Connection strings in `config.xml`
- API keys in app settings
- Log levels via environment variables

### Health Checks
- `/api/v1/health` - System health
- `/api/v1/system/status` - Application status
- `/api/v1/system/task` - Background tasks

## Contributing

### Pull Request Guidelines
1. Create feature branch from `develop`
2. Follow existing code style
3. Add tests for new functionality
4. Update documentation
5. Ensure all tests pass
6. Submit PR with clear description

### Code Review Checklist
- [ ] Follows architecture patterns
- [ ] Has appropriate tests
- [ ] Handles errors properly
- [ ] Includes logging
- [ ] Updates documentation
- [ ] No breaking changes

## Resources

### Documentation
- API Documentation: `/api/docs`
- Database Schema: `/docs/database.md`
- Architecture: `/docs/architecture.md`

### Tools
- Swagger UI: `/swagger`
- Database Browser: SQLite Browser
- API Testing: Postman/Insomnia

### Community
- GitHub Discussions
- Discord Server
- Community Forums
- Wiki