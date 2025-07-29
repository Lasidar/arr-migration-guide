# Migration Patterns Catalog

## Overview
This catalog documents reusable patterns discovered during the analysis of Sonarr → Lidarr → Readarr migrations. These patterns can guide future media type adaptations.

## Core Migration Patterns

### 1. Domain Model Transformation Pattern

**Intent**: Transform domain entities from one media type to another while preserving core functionality.

**Structure**:
```
Source Media Type          Target Media Type
├── Primary Entity    →    Primary Entity
├── Container Entity  →    Container Entity  
└── Atomic Unit      →    Atomic Unit
```

**Implementation**:

```csharp
// Step 1: Identify core concepts
TV Shows              Music                Books
Series          →     Artist         →     Author
Season          →     Album          →     Book
Episode         →     Track          →     Edition

// Step 2: Map relationships
Series.Seasons        Artist.Albums        Author.Books
Season.Episodes       Album.Tracks         Book.Editions

// Step 3: Preserve common properties
- Title/Name
- Overview/Description
- ReleaseDate
- Metadata IDs
- Quality profiles
```

**Example Usage**:
```csharp
// Before (Sonarr)
public class Series
{
    public string Title { get; set; }
    public List<Season> Seasons { get; set; }
}

// After (Lidarr)
public class Artist
{
    public string Name { get; set; }
    public LazyLoaded<List<Album>> Albums { get; set; }
}
```

### 2. Metadata Provider Adapter Pattern

**Intent**: Adapt external metadata providers to a common interface while handling media-specific requirements.

**Structure**:
```
IProvideMediaInfo
├── IProvideSeriesInfo (Sonarr)
├── IProvideArtistInfo (Lidarr)
└── IProvideAuthorInfo (Readarr)
```

**Implementation Steps**:

1. **Define media-specific interface**:
```csharp
public interface IProvide[MediaType]Info
{
    [MediaType] Get[MediaType]Info(string id);
    List<[MediaType]> Search[MediaType](string searchTerm);
}
```

2. **Create proxy implementation**:
```csharp
public class [MediaType]InfoProxy : IProvide[MediaType]Info
{
    private readonly IHttpClient _httpClient;
    private readonly I[MediaType]Mapper _mapper;
    
    public [MediaType] Get[MediaType]Info(string id)
    {
        var response = _httpClient.Get(BuildUrl(id));
        return _mapper.Map(response);
    }
}
```

3. **Map external IDs**:
```csharp
// Sonarr
TvdbId, TvRageId, ImdbId

// Lidarr  
MusicBrainzId, SpotifyId, DiscogsId

// Readarr
GoodreadsId, ISBN, ASIN
```

### 3. Search Criteria Abstraction Pattern

**Intent**: Create flexible search criteria that can be adapted to different media types and indexers.

**Structure**:
```
SearchCriteriaBase
├── SeriesSearchCriteria
├── AlbumSearchCriteria
└── BookSearchCriteria
```

**Implementation**:

```csharp
public abstract class SearchCriteriaBase
{
    public string Query { get; set; }
    public int? Year { get; set; }
    public abstract string GetSearchQuery();
}

public class BookSearchCriteria : SearchCriteriaBase
{
    public string Author { get; set; }
    public string ISBN { get; set; }
    
    public override string GetSearchQuery()
    {
        // Book-specific search logic
    }
}
```

### 4. Quality Definition Migration Pattern

**Intent**: Adapt quality definitions from one media type to another while maintaining upgrade/downgrade logic.

**Migration Steps**:

1. **Identify quality dimensions**:
   - TV: Resolution (720p, 1080p) + Source (HDTV, WEB-DL)
   - Music: Bitrate (MP3-320, FLAC) + Format (Lossy, Lossless)
   - Books: Format (EPUB, PDF) + Source (Retail, Web)

2. **Create quality hierarchy**:
```csharp
public class QualityDefinition
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Weight { get; set; } // For upgrade logic
}
```

3. **Map to numeric scores**:
```csharp
// Higher score = better quality
EPUB:  100
AZW3:  90
MOBI:  80
PDF:   70
```

### 5. File Organization Pattern

**Intent**: Create media-specific file naming and organization schemes.

**Pattern Variables**:

```csharp
// Base pattern
public abstract class NamingConfig
{
    protected Dictionary<string, Func<T, string>> TokenHandlers;
    
    public abstract string BuildFileName(T mediaItem);
}

// Media-specific implementation
public class BookNamingConfig : NamingConfig<Book>
{
    public BookNamingConfig()
    {
        TokenHandlers = new Dictionary<string, Func<Book, string>>
        {
            {"{Author Name}", b => b.Author.Name},
            {"{Book Title}", b => b.Title},
            {"{Release Year}", b => b.ReleaseDate?.Year.ToString()}
        };
    }
}
```

### 6. Import Decision Pattern

**Intent**: Make consistent import decisions across different media types.

**Structure**:
```
IImportDecisionEngineSpecification
├── AlreadyImportedSpecification
├── UpgradeSpecification
└── MediaSpecificSpecification
```

**Implementation**:
```csharp
public interface IImportDecisionEngineSpecification<T>
{
    Decision IsSatisfiedBy(T item, DownloadClientItem downloadClientItem);
}

public class BookUpgradeSpecification : IImportDecisionEngineSpecification<LocalBook>
{
    public Decision IsSatisfiedBy(LocalBook item, DownloadClientItem downloadClientItem)
    {
        // Check if this is an upgrade
        if (item.Book.BookFiles.Any())
        {
            var currentQuality = item.Book.BookFiles.First().Quality;
            return item.Quality > currentQuality 
                ? Decision.Accept() 
                : Decision.Reject("Not an upgrade");
        }
        return Decision.Accept();
    }
}
```

### 7. Event Translation Pattern

**Intent**: Translate generic events to media-specific events while maintaining consistent behavior.

**Implementation**:

```csharp
// Base event
public abstract class MediaAddedEvent<T> : IEvent
{
    public T Media { get; set; }
    public bool DoRefresh { get; set; }
}

// Media-specific events
public class SeriesAddedEvent : MediaAddedEvent<Series> { }
public class ArtistAddedEvent : MediaAddedEvent<Artist> { }
public class AuthorAddedEvent : MediaAddedEvent<Author> { }

// Handler pattern
public interface IHandle<TEvent> where TEvent : IEvent
{
    void Handle(TEvent message);
}
```

### 8. Repository Abstraction Pattern

**Intent**: Create consistent data access patterns across media types.

**Structure**:
```csharp
public interface IBasicRepository<TModel>
{
    TModel Get(int id);
    List<TModel> All();
    TModel Insert(TModel model);
    TModel Update(TModel model);
    void Delete(TModel model);
}

public interface IMediaRepository<TModel> : IBasicRepository<TModel>
{
    TModel FindByName(string name);
    List<TModel> GetByIds(List<int> ids);
}
```

### 9. Command Adaptation Pattern

**Intent**: Adapt background commands for media-specific operations.

**Template**:
```csharp
public abstract class RefreshMediaCommand<T> : Command
{
    public List<int> MediaIds { get; set; }
    
    public override bool SendUpdatesToClient => true;
}

public class RefreshAuthorCommand : RefreshMediaCommand<Author>
{
    public override string Name => "Refresh Author";
}
```

### 10. UI Component Migration Pattern

**Intent**: Create reusable UI components that can be adapted for different media types.

**Structure**:
```typescript
// Base component
interface MediaListProps<T> {
    items: T[];
    onEdit: (item: T) => void;
    onDelete: (item: T) => void;
}

// Media-specific implementation
const AuthorList: React.FC<MediaListProps<Author>> = (props) => {
    return <MediaList 
        {...props}
        columns={['name', 'bookCount', 'path']}
        itemComponent={AuthorListItem}
    />;
};
```

## Migration Checklist

When adapting to a new media type, follow this checklist:

### Phase 1: Domain Analysis
- [ ] Identify primary entity (creator/container)
- [ ] Identify collection entity
- [ ] Identify atomic unit (downloadable item)
- [ ] Map relationships between entities
- [ ] Identify unique metadata requirements

### Phase 2: Provider Integration
- [ ] Research available metadata providers
- [ ] Define provider interfaces
- [ ] Implement provider proxies
- [ ] Map external IDs
- [ ] Handle rate limiting

### Phase 3: Quality System
- [ ] Define quality dimensions
- [ ] Create quality hierarchy
- [ ] Implement upgrade logic
- [ ] Define default profiles

### Phase 4: Search and Download
- [ ] Adapt search criteria
- [ ] Implement indexer integration
- [ ] Define download specifications
- [ ] Handle media-specific packaging

### Phase 5: File Management
- [ ] Define naming patterns
- [ ] Implement organization logic
- [ ] Handle media-specific formats
- [ ] Create import specifications

### Phase 6: UI Adaptation
- [ ] Create media list views
- [ ] Adapt detail pages
- [ ] Update terminology
- [ ] Implement media-specific features

## Anti-Patterns to Avoid

### 1. Over-Generalization
**Problem**: Making abstractions too generic loses media-specific optimizations.
**Solution**: Balance abstraction with media-specific implementations.

### 2. Metadata Coupling
**Problem**: Tightly coupling to specific metadata providers.
**Solution**: Use adapter pattern with provider interfaces.

### 3. Quality Confusion
**Problem**: Trying to use same quality definitions across media types.
**Solution**: Create media-specific quality hierarchies.

### 4. Search Rigidity
**Problem**: Hard-coding search parameters.
**Solution**: Use flexible search criteria with media-specific extensions.

## Best Practices

1. **Start with the simplest media model** and add complexity as needed
2. **Preserve successful patterns** from parent projects
3. **Document media-specific decisions** for future reference
4. **Create integration tests** for provider interactions
5. **Use feature flags** for gradual rollout of new features
6. **Maintain backward compatibility** where possible
7. **Engage community early** for feedback on media-specific features

## Conclusion

These patterns provide a proven approach to adapting the *arr architecture to new media types. By following these patterns, developers can:

- Reduce development time
- Maintain consistency across projects
- Avoid common pitfalls
- Create maintainable solutions
- Enable future enhancements

The key to successful migration is understanding which patterns to apply and when to create media-specific solutions.