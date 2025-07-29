# Domain Model Evolution

## Executive Summary
This document traces the evolution of domain models from Sonarr (TV shows) through Lidarr (music) to Readarr (books), showing how core concepts were transformed and adapted for each media type.

## Core Entity Mapping

### Primary Entity Evolution

| Sonarr | Lidarr | Readarr | Purpose |
|--------|--------|---------|---------|
| Series | Artist | Author | Primary content creator/container |
| Season | Album | Book | Primary content collection |
| Episode | Track | Edition | Individual consumable unit |

### Entity Relationships

**Sonarr (TV):**
- Series → has many → Seasons
- Season → has many → Episodes
- Episodes are the downloadable units

**Lidarr (Music):**
- Artist → has many → Albums
- Album → has many → Tracks
- Album → has many → AlbumReleases (different versions)
- Tracks are grouped into releases for downloading

**Readarr (Books):**
- Author → has many → Books
- Book → has many → Editions
- Author → has many → Series (book series)
- Editions are the downloadable units

## Detailed Model Analysis

### Primary Entity (Series → Artist → Author)

**Series (Sonarr):**
```csharp
public class Series : ModelBase
{
    public int TvdbId { get; set; }
    public string Title { get; set; }
    public SeriesStatusType Status { get; set; }
    public string Overview { get; set; }
    public string Network { get; set; }
    public string AirTime { get; set; }
    public List<Season> Seasons { get; set; }
    // ... additional properties
}
```

**Artist (Lidarr):**
```csharp
public class Artist : Entity<Artist>
{
    public int ArtistMetadataId { get; set; }
    public string CleanName { get; set; }
    public string SortName { get; set; }
    public LazyLoaded<ArtistMetadata> Metadata { get; set; }
    public LazyLoaded<List<Album>> Albums { get; set; }
    // ... additional properties
}
```

**Author (Readarr):**
```csharp
public class Author : Entity<Author>
{
    public int AuthorMetadataId { get; set; }
    public string CleanName { get; set; }
    public LazyLoaded<AuthorMetadata> Metadata { get; set; }
    public LazyLoaded<List<Book>> Books { get; set; }
    public LazyLoaded<List<Series>> Series { get; set; }
    // ... additional properties
}
```

### Key Architectural Changes

1. **Metadata Separation (Lidarr/Readarr):**
   - Introduced separate metadata entities (ArtistMetadata, AuthorMetadata)
   - Allows for better data normalization and sharing
   - Not present in Sonarr's simpler model

2. **Entity Base Class Evolution:**
   - Sonarr: Inherits from `ModelBase`
   - Lidarr/Readarr: Inherit from `Entity<T>` with generic typing
   - Provides better type safety and standardized behavior

3. **Lazy Loading Introduction:**
   - Sonarr: Direct list properties
   - Lidarr/Readarr: `LazyLoaded<T>` wrapper for related entities
   - Improves performance for large libraries

### Secondary Entity (Season → Album → Book)

**Season (Sonarr):**
```csharp
public class Season : IEmbeddedDocument
{
    public int SeasonNumber { get; set; }
    public bool Monitored { get; set; }
    public List<MediaCover.MediaCover> Images { get; set; }
}
```

**Album (Lidarr):**
```csharp
public class Album : Entity<Album>
{
    public int ArtistMetadataId { get; set; }
    public string ForeignAlbumId { get; set; }
    public string Title { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public List<SecondaryAlbumType> SecondaryTypes { get; set; }
    public LazyLoaded<List<AlbumRelease>> AlbumReleases { get; set; }
    // ... additional properties
}
```

**Book (Readarr):**
```csharp
public class Book : Entity<Book>
{
    public int AuthorMetadataId { get; set; }
    public string ForeignBookId { get; set; }
    public string Title { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public List<int> RelatedBooks { get; set; }
    public LazyLoaded<List<Edition>> Editions { get; set; }
    public LazyLoaded<List<SeriesBookLink>> SeriesLinks { get; set; }
    // ... additional properties
}
```

### Major Differences:
1. **Complexity Growth:** Season is a simple embedded document, while Album and Book are full entities
2. **Release Variations:** Albums have AlbumReleases, Books have Editions
3. **Cross-References:** Books can belong to multiple series via SeriesLinks

## Metadata Provider Integration

### External ID Systems

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| TvdbId | MusicBrainzId | GoodreadsId |
| TvRageId | DiscogsId | OpenLibraryId |
| TvMazeId | SpotifyId | GoogleBooksId |
| ImdbId | LastFmId | IsbnId |
| TmdbId | | AsinId |

### Metadata Storage Evolution

**Sonarr:** Metadata stored directly on Series entity
**Lidarr/Readarr:** Separate metadata entities with foreign key relationships

Benefits of separated metadata:
- Easier updates from external sources
- Better handling of multiple identifiers
- Cleaner separation of concerns

## Quality and Format Handling

### Quality Profiles
- All three maintain quality profiles
- Media-specific quality definitions:
  - Sonarr: HDTV-720p, WEB-DL-1080p, Bluray-1080p
  - Lidarr: MP3-320, FLAC, AAC-256
  - Readarr: EPUB, MOBI, AZW3, PDF

### Metadata Profiles (Lidarr/Readarr only)
- Controls which releases/editions to prefer
- Not present in Sonarr
- Allows for:
  - Preferred album types (Studio, Live, Compilation)
  - Preferred book editions (First edition, Revised, Abridged)

## Monitoring and Download Decisions

### Monitoring Types
All three support:
- Individual item monitoring (episode/track/book)
- Bulk monitoring (season/album/author)
- Future item monitoring (new episodes/albums/books)

### Search Behavior
- **Sonarr:** Searches for complete seasons or individual episodes
- **Lidarr:** Searches for complete albums (all tracks in a release)
- **Readarr:** Searches for specific editions of books

## Database Schema Evolution

### Table Structure Changes

**Sonarr Core Tables:**
- Series
- Episodes
- EpisodeFiles
- History
- Blacklist

**Lidarr Core Tables:**
- Artists
- ArtistMetadata
- Albums
- AlbumReleases
- Tracks
- TrackFiles
- History
- Blacklist

**Readarr Core Tables:**
- Authors
- AuthorMetadata
- Books
- Editions
- BookFiles
- Series
- SeriesBookLinks
- History
- Blocklist

### Key Schema Insights:
1. Progressive complexity increase
2. Introduction of link tables for many-to-many relationships
3. Metadata separation pattern established in Lidarr
4. "Blacklist" renamed to "Blocklist" in Readarr

## Migration Patterns Identified

### 1. Entity Generalization Pattern
- Identify core concepts (creator, collection, item)
- Map domain-specific terms to generic concepts
- Maintain consistent relationships

### 2. Metadata Abstraction Pattern
- Separate volatile external metadata from stable internal data
- Use foreign key relationships for flexibility
- Implement lazy loading for performance

### 3. Quality Definition Pattern
- Define media-specific quality levels
- Map to generic quality score system
- Maintain upgrade/downgrade logic

### 4. Search and Download Pattern
- Abstract search criteria to base class
- Implement media-specific search strategies
- Maintain consistent download decision engine

### 5. File Management Pattern
- Generic file operations (move, rename, delete)
- Media-specific naming conventions
- Consistent organization preferences

## Recommendations for Future Media Types

Based on the evolution patterns observed:

1. **Start with Entity Mapping:**
   - Identify primary creator/container
   - Identify collection/grouping concept
   - Identify atomic downloadable unit

2. **Design Metadata Strategy:**
   - Identify external metadata sources
   - Plan for multiple provider support
   - Separate volatile from stable data

3. **Define Quality Hierarchy:**
   - Research media-specific quality factors
   - Map to numeric scoring system
   - Plan for future format support

4. **Plan Search Strategy:**
   - Understand how media is distributed
   - Identify search parameters
   - Design flexible search criteria

5. **Consider Special Requirements:**
   - Cross-references (like book series)
   - Multiple versions (like album releases)
   - Media-specific metadata (like ISBN, UPC)

## Conclusion

The evolution from Sonarr to Lidarr to Readarr demonstrates a maturing architecture that becomes more flexible and abstract with each iteration. Key improvements include:

1. Better separation of concerns
2. More flexible metadata handling
3. Improved performance through lazy loading
4. More sophisticated relationship modeling
5. Greater extensibility for future media types

These patterns provide a solid foundation for adapting the architecture to additional media types while maintaining consistency with the established ecosystem.