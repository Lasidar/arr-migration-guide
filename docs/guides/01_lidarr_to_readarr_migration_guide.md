# Lidarr to Readarr Migration Guide

This guide provides detailed instructions for migrating from Lidarr to Readarr, replicating the original migration process that occurred when Readarr was forked from Lidarr at commit `47f4441`. This guide is designed for senior developers who need to perform a similar migration on a more recent fork from Lidarr.

## General Migration Information

### Migration Overview

The migration from Lidarr to Readarr involves transforming a music-focused media management system into a book-focused system. The core architectural patterns remain the same, but the domain models and business logic are adapted for book management instead of music management.

### Key Migration Points

- **Domain Change**: Music (Artists/Albums/Tracks) → Books (Authors/Books/Editions)
- **Metadata Sources**: MusicBrainz → Goodreads, Google Books, BookInfo
- **File Management**: Audio files → Ebook files (EPUB, MOBI, PDF, etc.)
- **API Endpoints**: Music-focused → Book-focused
- **Database Schema**: Music tables → Book tables

### Migration Strategy

1. **Preserve Core Architecture**: Maintain the underlying .NET framework, database patterns, and API structure
2. **Transform Domain Models**: Replace music-specific models with book-specific equivalents
3. **Update Business Logic**: Adapt services and repositories for book management
4. **Modify API Layer**: Update controllers and resources for book operations
5. **Update Frontend**: Transform UI components for book management

## General Architectural Migration Information

### Core Architecture Preservation

The migration preserves the following architectural components:

- **Backend Framework**: .NET Core application structure
- **Database Layer**: SQLite with Dapper ORM
- **API Structure**: RESTful API with SignalR for real-time updates
- **Authentication**: API key and basic authentication
- **Configuration**: JSON-based configuration system
- **Logging**: NLog integration
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection

### Architectural Changes

#### 1. Domain Model Transformation

**Lidarr Domain Models:**
- `Artist` → `Author`
- `Album` → `Book`
- `AlbumRelease` → `Edition`
- `Track` → `BookFile`
- `ArtistMetadata` → `AuthorMetadata`

**Key Differences:**
- Books have editions (multiple formats/versions of the same book)
- Authors have different metadata requirements (birth/death dates, hometown, etc.)
- Book files are typically single files rather than multiple tracks

#### 2. Database Schema Changes

**Table Renaming:**
- `Artists` → `Authors`
- `Albums` → `Books`
- `AlbumReleases` → `Editions`
- `Tracks` → `BookFiles`
- `ArtistMetadata` → `AuthorMetadata`

**Schema Modifications:**
- Add book-specific fields (ISBN, page count, publisher, etc.)
- Remove music-specific fields (duration, album type, etc.)
- Update foreign key relationships

#### 3. API Structure Changes

**Namespace Changes:**
- `Lidarr.Api.V1` → `Readarr.Api.V1`
- `Lidarr.Http` → `Readarr.Http`

**Endpoint Adaptations:**
- `/api/v1/artist` → `/api/v1/author`
- `/api/v1/album` → `/api/v1/book`
- `/api/v1/track` → `/api/v1/bookfile`

## Implementation Detail Migration Information

### 1. Code Removal

#### Remove Music-Specific Components

**Delete these directories and files:**
```
src/NzbDrone.Core/Music/
src/Lidarr.Api.V1/
src/Lidarr.Http/
```

**Remove music-specific services:**
- `ArtistService` → Replace with `AuthorService`
- `AlbumService` → Replace with `BookService`
- `TrackService` → Replace with `BookFileService`

**Remove music-specific repositories:**
- `ArtistRepository` → Replace with `AuthorRepository`
- `AlbumRepository` → Replace with `BookRepository`
- `TrackRepository` → Replace with `BookFileRepository`

#### Remove Music-Specific Models

**Delete these model classes:**
- `Artist.cs`
- `Album.cs`
- `AlbumRelease.cs`
- `Track.cs`
- `ArtistMetadata.cs`
- `AddAlbumOptions.cs`
- `PrimaryAlbumType.cs`
- `SecondaryAlbumType.cs`

#### Remove Music-Specific API Resources

**Delete these API resource classes:**
- `ArtistResource.cs`
- `AlbumResource.cs`
- `AlbumReleaseResource.cs`
- `TrackFileResource.cs`
- `MediumResource.cs`

#### Remove Music-Specific Controllers

**Delete these controller classes:**
- `ArtistController.cs`
- `AlbumController.cs`
- `TrackFileController.cs`

### 2. Code Adaptation

#### Model Transformations

**Artist → Author Transformation:**

```csharp
// Lidarr Artist.cs
public class Artist : Entity<Artist>
{
    public int ArtistMetadataId { get; set; }
    public string CleanName { get; set; }
    public string SortName { get; set; }
    public bool Monitored { get; set; }
    public NewItemMonitorTypes MonitorNewItems { get; set; }
    public DateTime? LastInfoSync { get; set; }
    public string Path { get; set; }
    public string RootFolderPath { get; set; }
    public DateTime Added { get; set; }
    public int QualityProfileId { get; set; }
    public int MetadataProfileId { get; set; }
    public HashSet<int> Tags { get; set; }
    public LazyLoaded<ArtistMetadata> Metadata { get; set; }
    public LazyLoaded<QualityProfile> QualityProfile { get; set; }
    public LazyLoaded<MetadataProfile> MetadataProfile { get; set; }
    public LazyLoaded<List<Album>> Albums { get; set; }
}

// Readarr Author.cs
public class Author : Entity<Author>
{
    public int AuthorMetadataId { get; set; }
    public string CleanName { get; set; }
    public bool Monitored { get; set; }
    public NewItemMonitorTypes MonitorNewItems { get; set; }
    public DateTime? LastInfoSync { get; set; }
    public string Path { get; set; }
    public string RootFolderPath { get; set; }
    public DateTime Added { get; set; }
    public int QualityProfileId { get; set; }
    public int MetadataProfileId { get; set; }
    public HashSet<int> Tags { get; set; }
    public LazyLoaded<AuthorMetadata> Metadata { get; set; }
    public LazyLoaded<QualityProfile> QualityProfile { get; set; }
    public LazyLoaded<MetadataProfile> MetadataProfile { get; set; }
    public LazyLoaded<List<Book>> Books { get; set; }
    public LazyLoaded<List<Series>> Series { get; set; }
}
```

**Album → Book Transformation:**

```csharp
// Lidarr Album.cs
public class Album : Entity<Album>
{
    public int ArtistMetadataId { get; set; }
    public string ForeignAlbumId { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public string AlbumType { get; set; }
    public List<SecondaryAlbumType> SecondaryTypes { get; set; }
    public bool Monitored { get; set; }
    public bool AnyReleaseOk { get; set; }
    public LazyLoaded<List<AlbumRelease>> AlbumReleases { get; set; }
    public LazyLoaded<Artist> Artist { get; set; }
}

// Readarr Book.cs
public class Book : Entity<Book>
{
    public int AuthorMetadataId { get; set; }
    public string ForeignBookId { get; set; }
    public string ForeignEditionId { get; set; }
    public string TitleSlug { get; set; }
    public string Title { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public bool Monitored { get; set; }
    public bool AnyEditionOk { get; set; }
    public LazyLoaded<List<Edition>> Editions { get; set; }
    public LazyLoaded<Author> Author { get; set; }
    public LazyLoaded<List<BookFile>> BookFiles { get; set; }
}
```

#### Repository Transformations

**ArtistRepository → AuthorRepository:**

```csharp
// Lidarr ArtistRepository.cs
public class ArtistRepository : BasicRepository<Artist>, IArtistRepository
{
    public bool ArtistPathExists(string path)
    {
        return Query(c => c.Path == path).Any();
    }

    public Artist FindById(string foreignArtistId)
    {
        return Query(Builder().Where<ArtistMetadata>(m => m.ForeignArtistId == foreignArtistId)).SingleOrDefault();
    }

    public Dictionary<int, string> AllArtistPaths()
    {
        using (var conn = _database.OpenConnection())
        {
            var strSql = "SELECT \"Id\" AS \"Key\", \"Path\" AS \"Value\" FROM \"Artists\"";
            return conn.Query<KeyValuePair<int, string>>(strSql).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}

// Readarr AuthorRepository.cs
public class AuthorRepository : BasicRepository<Author>, IAuthorRepository
{
    public bool AuthorPathExists(string path)
    {
        return Query(c => c.Path == path).Any();
    }

    public Author FindById(string foreignAuthorId)
    {
        return Query(Builder().Where<AuthorMetadata>(m => m.ForeignAuthorId == foreignAuthorId)).SingleOrDefault();
    }

    public Dictionary<int, string> AllAuthorPaths()
    {
        using (var conn = _database.OpenConnection())
        {
            var strSql = "SELECT \"Id\" AS \"Key\", \"Path\" AS \"Value\" FROM \"Authors\"";
            return conn.Query<KeyValuePair<int, string>>(strSql).ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
```

#### Service Transformations

**ArtistService → AuthorService:**

```csharp
// Lidarr ArtistService.cs
public interface IArtistService
{
    Artist GetArtist(int artistId);
    Artist AddArtist(Artist newArtist, bool doRefresh);
    void DeleteArtist(int artistId, bool deleteFiles, bool addImportListExclusion = false);
    List<Artist> GetAllArtists();
    Artist UpdateArtist(Artist artist, bool publishUpdatedEvent = true);
    Dictionary<int, string> AllArtistPaths();
    bool ArtistPathExists(string folder);
}

// Readarr AuthorService.cs
public interface IAuthorService
{
    Author GetAuthor(int authorId);
    Author AddAuthor(Author newAuthor, bool doRefresh);
    void DeleteAuthor(int authorId, bool deleteFiles, bool addImportListExclusion = false);
    List<Author> GetAllAuthors();
    Author UpdateAuthor(Author author);
    Dictionary<int, string> AllAuthorPaths();
    bool AuthorPathExists(string folder);
}
```

#### API Resource Transformations

**ArtistResource → AuthorResource:**

```csharp
// Lidarr ArtistResource.cs
public class ArtistResource : RestResource
{
    public int ArtistMetadataId { get; set; }
    public ArtistStatusType Status { get; set; }
    public string ArtistName { get; set; }
    public string ForeignArtistId { get; set; }
    public string Overview { get; set; }
    public string ArtistType { get; set; }
    public string Disambiguation { get; set; }
    public List<Links> Links { get; set; }
    public AlbumResource NextAlbum { get; set; }
    public AlbumResource LastAlbum { get; set; }
    public List<MediaCover> Images { get; set; }
    public string Path { get; set; }
    public int QualityProfileId { get; set; }
    public int MetadataProfileId { get; set; }
    public bool Monitored { get; set; }
    public NewItemMonitorTypes MonitorNewItems { get; set; }
    public string RootFolderPath { get; set; }
    public List<string> Genres { get; set; }
    public string CleanName { get; set; }
    public string SortName { get; set; }
    public HashSet<int> Tags { get; set; }
    public DateTime Added { get; set; }
    public AddArtistOptions AddOptions { get; set; }
    public Ratings Ratings { get; set; }
    public ArtistStatisticsResource Statistics { get; set; }
}

// Readarr AuthorResource.cs
public class AuthorResource : RestResource
{
    public int AuthorMetadataId { get; set; }
    public AuthorStatusType Status { get; set; }
    public string AuthorName { get; set; }
    public string AuthorNameLastFirst { get; set; }
    public string ForeignAuthorId { get; set; }
    public string TitleSlug { get; set; }
    public string Overview { get; set; }
    public string Disambiguation { get; set; }
    public List<Links> Links { get; set; }
    public Book NextBook { get; set; }
    public Book LastBook { get; set; }
    public List<MediaCover> Images { get; set; }
    public string Path { get; set; }
    public int QualityProfileId { get; set; }
    public int MetadataProfileId { get; set; }
    public bool Monitored { get; set; }
    public NewItemMonitorTypes MonitorNewItems { get; set; }
    public string RootFolderPath { get; set; }
    public List<string> Genres { get; set; }
    public string CleanName { get; set; }
    public string SortName { get; set; }
    public string SortNameLastFirst { get; set; }
    public HashSet<int> Tags { get; set; }
    public DateTime Added { get; set; }
    public AddAuthorOptions AddOptions { get; set; }
    public Ratings Ratings { get; set; }
    public AuthorStatisticsResource Statistics { get; set; }
}
```

### 3. Database Migration

#### Create New Migration Files

**Initial Setup Migration:**

```csharp
// src/NzbDrone.Core/Datastore/Migration/001_initial_setup.cs
[Migration(1)]
public class InitialSetup : NzbDroneMigrationBase
{
    protected override void MainDbUpgrade()
    {
        Create.TableForModel("Config")
            .WithColumn("Key").AsString().Unique()
            .WithColumn("Value").AsString();

        Create.TableForModel("RootFolders")
            .WithColumn("Path").AsString().Unique()
            .WithColumn("Name").AsString().Nullable()
            .WithColumn("DefaultMetadataProfileId").AsInt32().WithDefaultValue(0)
            .WithColumn("DefaultQualityProfileId").AsInt32().WithDefaultValue(0)
            .WithColumn("DefaultMonitorOption").AsInt32().WithDefaultValue(0)
            .WithColumn("DefaultTags").AsString().Nullable()
            .WithColumn("IsCalibreLibrary").AsBoolean()
            .WithColumn("CalibreSettings").AsString().Nullable();

        Create.TableForModel("Authors")
            .WithColumn("CleanName").AsString().Indexed()
            .WithColumn("Path").AsString().Indexed()
            .WithColumn("Monitored").AsBoolean()
            .WithColumn("LastInfoSync").AsDateTime().Nullable()
            .WithColumn("SortName").AsString().Nullable()
            .WithColumn("QualityProfileId").AsInt32().Nullable()
            .WithColumn("Tags").AsString().Nullable()
            .WithColumn("Added").AsDateTime().Nullable()
            .WithColumn("AddOptions").AsString().Nullable()
            .WithColumn("MetadataProfileId").AsInt32().WithDefaultValue(1)
            .WithColumn("AuthorMetadataId").AsInt32().Unique();

        Create.TableForModel("AuthorMetadata")
            .WithColumn("ForeignAuthorId").AsString().Unique()
            .WithColumn("TitleSlug").AsString().Unique()
            .WithColumn("Name").AsString()
            .WithColumn("Overview").AsString().Nullable()
            .WithColumn("Disambiguation").AsString().Nullable()
            .WithColumn("Gender").AsString().Nullable()
            .WithColumn("Hometown").AsString().Nullable()
            .WithColumn("Born").AsDateTime().Nullable()
            .WithColumn("Died").AsDateTime().Nullable()
            .WithColumn("Status").AsInt32()
            .WithColumn("Images").AsString()
            .WithColumn("Links").AsString().Nullable()
            .WithColumn("Genres").AsString().Nullable()
            .WithColumn("Ratings").AsString().Nullable()
            .WithColumn("Aliases").AsString().WithDefaultValue("[]");

        Create.TableForModel("Books")
            .WithColumn("AuthorMetadataId").AsInt32().WithDefaultValue(0)
            .WithColumn("ForeignBookId").AsString().Indexed()
            .WithColumn("TitleSlug").AsString().Unique()
            .WithColumn("Title").AsString()
            .WithColumn("ReleaseDate").AsDateTime().Nullable()
            .WithColumn("Links").AsString().Nullable()
            .WithColumn("Genres").AsString().Nullable()
            .WithColumn("Ratings").AsString().Nullable()
            .WithColumn("CleanTitle").AsString().Indexed()
            .WithColumn("Monitored").AsBoolean()
            .WithColumn("AnyEditionOk").AsBoolean()
            .WithColumn("LastInfoSync").AsDateTime().Nullable()
            .WithColumn("Added").AsDateTime().Nullable()
            .WithColumn("AddOptions").AsString().Nullable();

        Create.TableForModel("Editions")
            .WithColumn("BookId").AsInt32().WithDefaultValue(0)
            .WithColumn("ForeignEditionId").AsString().Unique()
            .WithColumn("Isbn13").AsString().Nullable()
            .WithColumn("Asin").AsString().Nullable()
            .WithColumn("Title").AsString()
            .WithColumn("TitleSlug").AsString()
            .WithColumn("Language").AsString().Nullable()
            .WithColumn("Overview").AsString().Nullable()
            .WithColumn("Format").AsString().Nullable()
            .WithColumn("IsEbook").AsBoolean().Nullable()
            .WithColumn("Disambiguation").AsString().Nullable()
            .WithColumn("Publisher").AsString().Nullable()
            .WithColumn("PageCount").AsInt32().Nullable()
            .WithColumn("ReleaseDate").AsDateTime().Nullable()
            .WithColumn("Images").AsString()
            .WithColumn("Links").AsString().Nullable()
            .WithColumn("Ratings").AsString().Nullable()
            .WithColumn("Monitored").AsBoolean()
            .WithColumn("ManualAdd").AsBoolean();

        Create.TableForModel("BookFiles")
            .WithColumn("EditionId").AsInt32().Indexed()
            .WithColumn("CalibreId").AsInt32()
            .WithColumn("Quality").AsString()
            .WithColumn("Size").AsInt64()
            .WithColumn("SceneName").AsString().Nullable()
            .WithColumn("DateAdded").AsDateTime()
            .WithColumn("ReleaseGroup").AsString().Nullable()
            .WithColumn("MediaInfo").AsString().Nullable()
            .WithColumn("Modified").AsDateTime().WithDefaultValue(new DateTime(2000, 1, 1))
            .WithColumn("Path").AsString().NotNullable().Unique();
    }
}
```

#### Update Table Mapping

```csharp
// src/NzbDrone.Core/Datastore/TableMapping.cs
Mapper.Entity<Author>("Authors")
    .Ignore(s => s.RootFolderPath)
    .Ignore(s => s.Name)
    .Ignore(s => s.ForeignAuthorId)
    .HasOne(a => a.Metadata, a => a.AuthorMetadataId)
    .HasOne(a => a.QualityProfile, a => a.QualityProfileId)
    .HasOne(s => s.MetadataProfile, s => s.MetadataProfileId)
    .LazyLoad(a => a.Books, (db, a) => db.Query<Book>(new SqlBuilder(db.DatabaseType).Where<Book>(b => b.AuthorMetadataId == a.AuthorMetadataId)).ToList(), a => a.AuthorMetadataId > 0);

Mapper.Entity<Book>("Books").RegisterModel()
    .Ignore(x => x.AuthorId)
    .Ignore(x => x.ForeignEditionId)
    .HasOne(r => r.AuthorMetadata, r => r.AuthorMetadataId)
    .LazyLoad(x => x.BookFiles,
              (db, book) => db.Query<BookFile>(new SqlBuilder(db.DatabaseType)
                                               .Join<BookFile, Edition>((l, r) => l.EditionId == r.Id)
                                               .Where<Edition>(b => b.BookId == book.Id)).ToList(),
              b => b.Id > 0)
    .LazyLoad(x => x.Editions,
              (db, book) => db.Query<Edition>(new SqlBuilder(db.DatabaseType).Where<Edition>(e => e.BookId == book.Id)).ToList(),
              b => b.Id > 0)
    .LazyLoad(a => a.Author,
              (db, book) => AuthorRepository.Query(db,
                                                    new SqlBuilder(db.DatabaseType)
                                                    .Join<Author, AuthorMetadata>((a, m) => a.AuthorMetadataId == m.Id)
                                                    .Where<Author>(a => a.AuthorMetadataId == book.AuthorMetadataId)).SingleOrDefault(),
              a => a.AuthorMetadataId > 0);
```

### 4. Metadata Source Adaptation

#### Replace MusicBrainz with Goodreads/Google Books

**Remove MusicBrainz Integration:**
- Delete `NzbDrone.Core.MetadataSource.SkyHook/`
- Remove MusicBrainz API calls

**Add Book Metadata Sources:**

```csharp
// src/NzbDrone.Core/MetadataSource/Goodreads/
public class GoodreadsProxy : IMetadataSource
{
    public Author GetAuthorInfo(string foreignAuthorId)
    {
        // Implementation for Goodreads author lookup
    }

    public Book GetBookInfo(string foreignBookId)
    {
        // Implementation for Goodreads book lookup
    }
}

// src/NzbDrone.Core/MetadataSource/GoogleBooks/
public class GoogleBooksProxy : IMetadataSource
{
    public Author GetAuthorInfo(string foreignAuthorId)
    {
        // Implementation for Google Books author lookup
    }

    public Book GetBookInfo(string foreignBookId)
    {
        // Implementation for Google Books book lookup
    }
}
```

### 5. Parser Adaptation

#### Update Parsed Models

```csharp
// src/NzbDrone.Core/Parser/Model/ParsedBookInfo.cs
public class ParsedBookInfo
{
    public string BookTitle { get; set; }
    public string AuthorName { get; set; }
    public AuthorTitleInfo AuthorTitleInfo { get; set; }
    public QualityModel Quality { get; set; }
    public string ReleaseDate { get; set; }
    public bool Discography { get; set; }
    public int DiscographyStart { get; set; }
    public int DiscographyEnd { get; set; }
    public string ReleaseGroup { get; set; }
    public string ReleaseHash { get; set; }
    public string ReleaseVersion { get; set; }
    public string ReleaseTitle { get; set; }

    [JsonIgnore]
    public Dictionary<string, object> ExtraInfo { get; set; } = new Dictionary<string, object>();
}
```

### 6. Frontend Adaptation

#### Update API Endpoints

**Replace Lidarr API calls with Readarr equivalents:**

```javascript
// Lidarr API calls
const artist = await api.get(`/api/v1/artist/${id}`);
const albums = await api.get(`/api/v1/album?artistId=${artistId}`);

// Readarr API calls
const author = await api.get(`/api/v1/author/${id}`);
const books = await api.get(`/api/v1/book?authorId=${authorId}`);
```

#### Update Component Names

**Rename React components:**
- `Artist` → `Author`
- `Album` → `Book`
- `TrackFile` → `BookFile`

#### Update TypeScript Interfaces

```typescript
// Lidarr interfaces
interface Artist {
  id: number;
  artistName: string;
  foreignArtistId: string;
  monitored: boolean;
  path: string;
  qualityProfileId: number;
  metadataProfileId: number;
  tags: number[];
}

// Readarr interfaces
interface Author {
  id: number;
  authorName: string;
  authorNameLastFirst: string;
  foreignAuthorId: string;
  titleSlug: string;
  monitored: boolean;
  path: string;
  qualityProfileId: number;
  metadataProfileId: number;
  tags: number[];
}
```

## Other Relevant Implementation Details

### 1. Configuration Updates

#### Update App Configuration

```json
// appsettings.json
{
  "App": {
    "Name": "Readarr",
    "Version": "1.0.0",
    "BuildInfo": {
      "Version": "1.0.0",
      "Branch": "main",
      "Commit": "development"
    }
  }
}
```

#### Update Build Configuration

```xml
<!-- src/Readarr.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyTitle>Readarr</AssemblyTitle>
    <AssemblyDescription>Readarr - Book Management</AssemblyDescription>
    <Product>Readarr</Product>
    <Company>Readarr Team</Company>
    <Copyright>Copyright © Readarr Team</Copyright>
  </PropertyGroup>
</Project>
```

### 2. Logging Updates

#### Update Log Messages

```csharp
// Replace Lidarr log messages
_logger.Info("Artist {0} added", artist.Name);

// With Readarr log messages
_logger.Info("Author {0} added", author.Name);
```

### 3. Notification Updates

#### Update Webhook Payloads

```csharp
// Lidarr webhook
public class WebhookArtist
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string MBId { get; set; }
    public string Type { get; set; }
    public string Overview { get; set; }
    public List<string> Genres { get; set; }
    public List<WebhookImage> Images { get; set; }
    public List<string> Tags { get; set; }
}

// Readarr webhook
public class WebhookAuthor
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string GoodreadsId { get; set; }
}
```

### 4. Quality Profile Updates

#### Update Quality Definitions

```csharp
// Remove music-specific qualities
public enum QualitySource
{
    Unknown = 0,
    Book = 1,  // Replace Track
    Ebook = 2, // Replace CD
    Audiobook = 3 // Replace Vinyl
}
```

### 5. Import List Updates

#### Update Import List Types

```csharp
// Replace music import lists
public enum ImportListType
{
    Goodreads = 1,    // Replace LastFM
    GoogleBooks = 2,  // Replace Spotify
    BookInfo = 3      // Replace MusicBrainz
}
```

### 6. Testing Updates

#### Update Test Data

```csharp
// Lidarr test data
var artist = new Artist
{
    Name = "Test Artist",
    ForeignArtistId = "test-artist-id"
};

// Readarr test data
var author = new Author
{
    Name = "Test Author",
    ForeignAuthorId = "test-author-id"
};
```

### 7. Documentation Updates

#### Update API Documentation

```yaml
# openapi.json
info:
  title: Readarr API
  description: Readarr API for book management
  version: 1.0.0
paths:
  /api/v1/author:
    get:
      summary: Get all authors
  /api/v1/book:
    get:
      summary: Get all books
```

### 8. Docker Configuration

#### Update Docker Labels

```dockerfile
# Dockerfile
LABEL maintainer="Readarr Team"
LABEL description="Readarr - Book Management"
LABEL org.opencontainers.image.title="Readarr"
LABEL org.opencontainers.image.description="Readarr - Book Management"
```

### 9. Package Configuration

#### Update Package Names

```json
// package.json
{
  "name": "readarr",
  "description": "Readarr - Book Management",
  "version": "1.0.0"
}
```

## Migration Checklist

### Pre-Migration Tasks
- [ ] Backup existing Lidarr database
- [ ] Document current Lidarr configuration
- [ ] Set up development environment
- [ ] Create new Git repository for Readarr

### Core Migration Tasks
- [ ] Create new domain models (Author, Book, Edition, etc.)
- [ ] Update database schema and migrations
- [ ] Transform repositories and services
- [ ] Update API controllers and resources
- [ ] Replace metadata sources
- [ ] Update parser logic

### Frontend Migration Tasks
- [ ] Update API endpoints
- [ ] Rename React components
- [ ] Update TypeScript interfaces
- [ ] Modify UI components for book management
- [ ] Update routing and navigation

### Post-Migration Tasks
- [ ] Update configuration files
- [ ] Modify logging and notifications
- [ ] Update documentation
- [ ] Create migration scripts
- [ ] Test all functionality
- [ ] Update deployment configurations

## Common Pitfalls and Solutions

### 1. Database Migration Issues

**Problem**: Foreign key constraints fail during migration
**Solution**: Ensure proper migration order and handle existing data

### 2. API Compatibility Issues

**Problem**: Frontend breaks due to API changes
**Solution**: Maintain backward compatibility or provide migration scripts

### 3. Metadata Source Integration

**Problem**: New metadata sources don't provide expected data
**Solution**: Implement fallback mechanisms and data transformation

### 4. File Management Changes

**Problem**: Book files handled differently than music files
**Solution**: Adapt file processing logic for book formats

### 5. Performance Issues

**Problem**: New queries perform poorly
**Solution**: Optimize database queries and add appropriate indexes

## Conclusion

This migration guide provides a comprehensive roadmap for transforming Lidarr into Readarr. The key is to maintain the core architectural patterns while adapting the domain models and business logic for book management. By following this guide systematically, developers can successfully replicate the original migration process and create a robust book management system based on the proven Lidarr architecture.

Remember to test thoroughly at each step and maintain backward compatibility where possible. The migration process should be iterative, with each component being migrated and tested before moving to the next. 