# Integration Mapping

## Overview
This document maps the evolution of external service integrations from Sonarr through Lidarr to Readarr, showing how each project adapted integrations for their specific media type.

## Metadata Providers

### Provider Interface Evolution

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| `IProvideSeriesInfo` | `IProvideArtistInfo` | `IProvideAuthorInfo` |
| - | `IProvideAlbumInfo` | `IProvideBookInfo` |
| `ISearchForNewSeries` | `ISearchForNewArtist` | `ISearchForNewAuthor` |
| - | `ISearchForNewAlbum` | `ISearchForNewBook` |

### Primary Metadata Sources

**Sonarr:**
- **Primary**: TheTVDB (via SkyHook proxy)
- **Secondary**: TVMaze, TMDB
- **Additional**: IMDB (for cross-referencing)

**Lidarr:**
- **Primary**: MusicBrainz (via SkyHook proxy)
- **Secondary**: Last.fm, Discogs
- **Additional**: Spotify (for additional metadata)

**Readarr:**
- **Primary**: Goodreads (via BookInfo proxy)
- **Secondary**: OpenLibrary, Google Books
- **Additional**: ISBN database

### Metadata Provider Implementation Pattern

```csharp
// Sonarr
public class SkyHookProxy : IProvideSeriesInfo, ISearchForNewSeries

// Lidarr
public class SkyHookProxy : IProvideArtistInfo, ISearchForNewArtist, 
                           IProvideAlbumInfo, ISearchForNewAlbum

// Readarr
public class BookInfoProxy : IProvideAuthorInfo, IProvideBookInfo, 
                            ISearchForNewBook, ISearchForNewAuthor
```

## Download Clients

### Common Download Clients (All Projects)
- **Usenet Clients:**
  - SABnzbd
  - NZBGet
  - Download Station (Synology)

- **Torrent Clients:**
  - qBittorrent
  - Deluge
  - Transmission
  - uTorrent
  - rTorrent/ruTorrent

### Download Client Architecture
All three projects share the same download client architecture:
- Base interface: `IDownloadClient`
- Common item model: `DownloadClientItem`
- Consistent status tracking and history management

### Key Differences in Download Handling

**Sonarr:**
- Handles individual episodes or complete seasons
- Season pack detection and handling
- Multi-episode file support

**Lidarr:**
- Downloads complete albums (all tracks)
- Handles various release formats (single, EP, album)
- Multi-disc album support

**Readarr:**
- Downloads individual book editions
- Handles multiple file formats per book
- Audiobook support considerations

## Indexers

### Indexer Types Supported

All projects support:
- **Newznab**: Generic indexer protocol
- **Torznab**: Torrent indexer protocol
- **RSS**: Generic RSS feeds
- **Custom**: Project-specific indexers

### Search Parameter Evolution

**Sonarr Search Parameters:**
- `tvdbid`, `tvmazeid`, `imdbid`
- `season`, `ep` (episode)
- Series title, year

**Lidarr Search Parameters:**
- `artist`, `album`
- `musicbrainzid`
- Release year, label

**Readarr Search Parameters:**
- `author`, `title`
- `goodreadsid`, `isbn`
- Publication year

## Notification Systems

### Common Notification Types
All projects support:
- Email
- Discord
- Slack
- Webhook
- Plex Media Server
- Emby/Jellyfin
- Custom Scripts

### Notification Event Evolution

**Sonarr Events:**
- On Grab (episode grabbed)
- On Download (episode imported)
- On Rename
- On Series Delete
- On Episode File Delete

**Lidarr Events:**
- On Grab (album grabbed)
- On Release Import
- On Rename
- On Artist Delete
- On Album Delete

**Readarr Events:**
- On Grab (book grabbed)
- On Import
- On Rename
- On Author Delete
- On Book Delete
- On Book Retag

### Metadata Link Types

**Sonarr:**
```csharp
public enum MetadataLinkType
{
    Tvdb = 1,
    Tvmaze = 2,
    Trakt = 3,
    Tmdb = 4,
    Imdb = 5
}
```

**Lidarr:**
```csharp
public enum MetadataLinkType
{
    MusicBrainz = 1,
    LastFm = 2,
    Discogs = 3,
    Spotify = 4
}
```

**Readarr:**
```csharp
public enum MetadataLinkType
{
    Goodreads = 1,
    OpenLibrary = 2,
    GoogleBooks = 3,
    Isbn = 4
}
```

## Import Lists

### Import List Evolution

**Sonarr Import Lists:**
- Trakt Lists
- IMDB Lists
- Sonarr (other instances)
- Plex Watchlist
- Custom Lists

**Lidarr Import Lists:**
- Spotify Playlists
- Last.fm User/Tags
- MusicBrainz Series
- Lidarr (other instances)
- Headphones

**Readarr Import Lists:**
- Goodreads Lists/Shelves
- LazyLibrarian
- Readarr (other instances)
- Goodreads Series

### Import List Architecture Pattern
```csharp
// Common base for all import lists
public abstract class ImportListBase<TSettings> : IImportList
    where TSettings : IProviderConfig, new()
{
    // Common implementation
}

// Media-specific implementations
public class GoodreadsListImportList : ImportListBase<GoodreadsListSettings>
public class SpotifyPlaylistImportList : ImportListBase<SpotifySettings>
public class TraktListImportList : ImportListBase<TraktSettings>
```

## Media Management

### File Organization Patterns

**Sonarr:**
- Series Folder: `{Series Title} ({Year})`
- Season Folder: `Season {season:00}`
- Episode File: `{Series Title} - S{season:00}E{episode:00} - {Episode Title}`

**Lidarr:**
- Artist Folder: `{Artist Name}`
- Album Folder: `{Artist Name} - {Album Title} ({Release Year})`
- Track File: `{track:00} - {Track Title}`

**Readarr:**
- Author Folder: `{Author Name}`
- Book File: `{Author Name} - {Book Title} ({Release Year})`
- Edition handling in filename

### Quality Definitions

**Sonarr Quality Types:**
- Resolution based: SDTV, HDTV-720p, HDTV-1080p, WEB-DL, Bluray
- Source based: HDTV, WEB-DL, Bluray

**Lidarr Quality Types:**
- Lossy: MP3-192, MP3-256, MP3-320, AAC-256
- Lossless: FLAC, ALAC
- Other: OGG, WMA

**Readarr Quality Types:**
- Text: EPUB, MOBI, AZW3, PDF, TXT
- Audio: MP3, M4B (audiobooks)
- Quality score based on format preference

## Integration Patterns

### 1. Provider Abstraction Pattern
- Define interface for media-specific operations
- Implement concrete providers for each service
- Use dependency injection for flexibility

### 2. Search Adaptation Pattern
- Common search criteria base class
- Media-specific search parameters
- Provider-specific query builders

### 3. Download Decision Pattern
- Shared decision engine interface
- Media-specific specifications
- Quality and format preferences

### 4. File Management Pattern
- Common file operations
- Media-specific naming conventions
- Format-specific handling

### 5. Notification Payload Pattern
- Base notification data
- Media-specific enrichment
- Provider-specific formatting

## Migration Guidelines

### When Adding New Integration Types

1. **Identify Integration Category:**
   - Metadata provider
   - Download client
   - Indexer
   - Notification
   - Import list

2. **Follow Established Patterns:**
   - Inherit from appropriate base class
   - Implement required interfaces
   - Use existing models where possible

3. **Consider Media-Specific Needs:**
   - Unique identifiers (ISBN, MusicBrainz ID, etc.)
   - Special metadata fields
   - Format-specific handling

4. **Maintain Consistency:**
   - Similar configuration UI
   - Consistent error handling
   - Shared validation logic

### Integration Testing Approach

1. **Mock External Services:**
   - Create test fixtures for API responses
   - Handle rate limiting scenarios
   - Test error conditions

2. **Validate Data Mapping:**
   - Ensure all fields map correctly
   - Handle missing/optional data
   - Validate data transformations

3. **Test Search Scenarios:**
   - Exact matches
   - Fuzzy searches
   - No results handling

## Conclusion

The integration architecture demonstrates:

1. **Consistent Patterns**: All three projects follow similar integration patterns
2. **Media-Specific Adaptations**: Each project adapts integrations for its media type
3. **Shared Infrastructure**: Download clients and core functionality remain consistent
4. **Extensibility**: New providers can be added following established patterns
5. **Maintainability**: Clear separation between shared and media-specific code

This architecture allows for:
- Easy addition of new providers
- Consistent user experience across projects
- Code reuse where appropriate
- Media-specific optimizations where needed