# Architecture Comparison: Sonarr vs Lidarr vs Readarr

## Overview
This document provides a comprehensive side-by-side comparison of the architectural evolution from Sonarr through Lidarr to Readarr.

## High-Level Architecture

### Common Architecture Pattern
All three projects follow the same fundamental architecture:
- **Frontend**: React-based SPA
- **Backend**: .NET Core/Framework API
- **Database**: SQLite with migrations
- **Communication**: SignalR for real-time updates
- **Background Jobs**: Task scheduling system

## API Architecture Evolution

### API Versioning Strategy

| Project | API Versions | Approach |
|---------|--------------|----------|
| Sonarr | V3, V5 | Multiple versions for backward compatibility |
| Lidarr | V1 | Single version, fresh start |
| Readarr | V1 | Single version, following Lidarr pattern |

### Core API Controllers

**Common Controllers (All Projects):**
- `CommandController` - Background task execution
- `ConfigController` - Application configuration
- `CustomFormatController` - Custom quality formats
- `DownloadClientController` - Download client management
- `HealthController` - System health checks
- `IndexerController` - Indexer management
- `QueueController` - Download queue management
- `RootFolderController` - Media library paths
- `TagController` - Tag management

**Media-Specific Controllers:**

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| `SeriesController` | `ArtistController` | `AuthorController` |
| `EpisodeController` | `AlbumController` | `BookController` |
| `SeasonPassController` | `AlbumStudioController` | `BookshelfController` |
| `EpisodeFileController` | `TrackFileController` | `BookFileController` |

### REST Controller Pattern Evolution

**Base Classes:**
```csharp
// Common base controller
public abstract class RestController<TResource>

// With SignalR support
public abstract class RestControllerWithSignalR<TResource, TModel> 
    : RestController<TResource>, IHandle<ModelEvent<TModel>>
```

## Service Layer Architecture

### Core Services Comparison

| Service Type | Sonarr | Lidarr | Readarr |
|--------------|--------|--------|---------|
| **Add Service** | `AddSeriesService` | `AddArtistService` | `AddAuthorService` |
| **Refresh Service** | `RefreshSeriesService` | `RefreshArtistService` | `RefreshAuthorService` |
| **Search Service** | `SeriesSearchService` | `ArtistSearchService` | `AuthorSearchService` |
| **Import Service** | `EpisodeImportService` | `TrackImportService` | `BookImportService` |
| **File Service** | `EpisodeFileService` | `TrackFileService` | `BookFileService` |

### Service Dependencies

**Common Pattern:**
```csharp
public class MediaService : IMediaService
{
    private readonly IMediaRepository _repository;
    private readonly IProvideMediaInfo _mediaInfo;
    private readonly IEventAggregator _eventAggregator;
    private readonly IConfigService _configService;
    // ... additional dependencies
}
```

## Data Access Layer

### Repository Pattern Evolution

**Sonarr:**
```csharp
public class SeriesRepository : BasicRepository<Series>, ISeriesRepository
{
    // Direct database access
}
```

**Lidarr/Readarr:**
```csharp
public class ArtistRepository : BasicRepository<Artist>, IArtistRepository
{
    // Enhanced with metadata relationships
}
```

### Database Migration Strategy

| Aspect | Sonarr | Lidarr | Readarr |
|--------|--------|---------|---------|
| **Migration Framework** | FluentMigrator | FluentMigrator | FluentMigrator |
| **Initial Migration** | 001 | 001 (fresh) | 001 (fresh) |
| **Migration Count** | 200+ | 70+ | 40+ |
| **Schema Complexity** | Moderate | High | Highest |

## Event-Driven Architecture

### Event Types Comparison

**Common Events:**
- `ModelEvent<T>` - Generic model updates
- `CommandExecutedEvent` - Background task completion
- `HealthCheckCompleteEvent` - System health updates

**Media-Specific Events:**

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| `SeriesAddedEvent` | `ArtistAddedEvent` | `AuthorAddedEvent` |
| `SeriesDeletedEvent` | `ArtistDeletedEvent` | `AuthorDeletedEvent` |
| `EpisodeImportedEvent` | `AlbumImportedEvent` | `BookImportedEvent` |
| `SeasonSearchEvent` | `AlbumSearchEvent` | `BookSearchEvent` |

### Event Aggregator Pattern
```csharp
public interface IEventAggregator
{
    void PublishEvent<TEvent>(TEvent @event) where TEvent : class, IEvent;
}
```

## Background Job Architecture

### Command Pattern Implementation

**Common Commands:**
- `ApplicationUpdateCommand`
- `BackupCommand`
- `CleanUpRecycleBinCommand`
- `HousekeepingCommand`
- `RefreshMonitoredDownloadsCommand`

**Media-Specific Commands:**

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| `RefreshSeriesCommand` | `RefreshArtistCommand` | `RefreshAuthorCommand` |
| `SeasonSearchCommand` | `AlbumSearchCommand` | `BookSearchCommand` |
| `SeriesSearchCommand` | `ArtistSearchCommand` | `AuthorSearchCommand` |

## Dependency Injection

### Container Registration Pattern

**Sonarr:**
```csharp
container.Register<ISeriesService, SeriesService>();
container.Register<IEpisodeService, EpisodeService>();
```

**Lidarr:**
```csharp
container.Register<IArtistService, ArtistService>();
container.Register<IAlbumService, AlbumService>();
container.Register<ITrackService, TrackService>();
```

**Readarr:**
```csharp
container.Register<IAuthorService, AuthorService>();
container.Register<IBookService, BookService>();
container.Register<IEditionService, EditionService>();
```

## Frontend Architecture

### Component Structure

**Common Components:**
- Calendar view
- Queue management
- System settings
- Activity/History

**Media-Specific Components:**

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| Series list | Artist list | Author list |
| Season pass | Album studio | Bookshelf |
| Episode details | Track list | Edition selector |

### State Management
- All use Redux for state management
- Similar action/reducer patterns
- Media-specific state slices

## Quality and Profile System

### Quality Profile Architecture

**Evolution:**
1. **Sonarr**: Simple quality profiles (resolution/source)
2. **Lidarr**: Added metadata profiles (release type preferences)
3. **Readarr**: Enhanced with format preferences (ebook formats)

### Custom Format Engine
- Introduced in Sonarr V3
- Adopted and enhanced in Lidarr
- Further refined in Readarr

## File Organization

### Naming Convention Engine

**Pattern Variables:**

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| `{Series Title}` | `{Artist Name}` | `{Author Name}` |
| `{season:00}` | `{Album Title}` | `{Book Title}` |
| `{episode:00}` | `{track:00}` | `{Edition Format}` |
| `{Episode Title}` | `{Track Title}` | `{Book Year}` |

## Performance Optimizations

### Lazy Loading Introduction

**Sonarr**: Direct property loading
```csharp
public List<Season> Seasons { get; set; }
```

**Lidarr/Readarr**: Lazy loading pattern
```csharp
public LazyLoaded<List<Album>> Albums { get; set; }
```

### Caching Strategy

| Layer | Sonarr | Lidarr | Readarr |
|-------|--------|---------|---------|
| **API Response** | Basic | Enhanced | Enhanced |
| **Database Query** | Simple | Optimized | Highly Optimized |
| **Metadata** | In-memory | Distributed | Distributed |

## Security Architecture

### Authentication Methods
- All support Forms authentication
- API key authentication
- Optional basic auth

### Authorization
- Role-based access (planned)
- API endpoint permissions
- UI feature toggles

## Monitoring and Logging

### Logging Framework
- All use NLog
- Structured logging
- File and console targets

### Health Checks

**Common Checks:**
- Database connectivity
- Disk space
- Download client status
- Indexer availability

**Media-Specific Checks:**
- Metadata provider availability
- Root folder accessibility
- Import mechanism health

## Deployment Architecture

### Platform Support
- Windows (Service/Tray)
- Linux (systemd)
- macOS
- Docker
- Kubernetes (community)

### Update Mechanism
- Built-in updater
- Branch selection (main/develop)
- Automatic backup before update

## Key Architectural Improvements

### Sonarr → Lidarr
1. Metadata abstraction layer
2. Enhanced provider interfaces
3. Improved lazy loading
4. Better separation of concerns

### Lidarr → Readarr
1. Further metadata refinements
2. Enhanced relationship modeling
3. Improved search algorithms
4. Better format handling

## Architecture Decision Records (ADRs)

### Key Decisions Made

1. **Single API Version (Lidarr/Readarr)**
   - Simplified maintenance
   - Cleaner codebase
   - Better for new projects

2. **Metadata Separation**
   - Easier updates
   - Better normalization
   - Cleaner domain models

3. **Lazy Loading**
   - Improved performance
   - Reduced memory usage
   - Better scalability

4. **Event-Driven Updates**
   - Real-time UI updates
   - Decoupled components
   - Better testability

## Conclusion

The architectural evolution shows:
1. **Consistent Core**: Fundamental architecture remains stable
2. **Progressive Enhancement**: Each fork improves on the previous
3. **Domain Adaptation**: Architecture adapts to media-specific needs
4. **Maintainable Design**: Clear patterns for future extensions

This architecture provides a solid foundation for:
- Future media type adaptations
- Performance improvements
- Feature additions
- Community contributions