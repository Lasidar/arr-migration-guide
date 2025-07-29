# Database Migration Analysis

## Overview
This document analyzes the database migration patterns across Sonarr, Lidarr, and Readarr, showing how the schema evolved to support different media types.

## Migration Statistics

| Project | Migration Count | Starting Point | Complexity |
|---------|----------------|----------------|------------|
| Sonarr | 260 | Original schema | High (evolved over time) |
| Lidarr | 96 | Fresh start | Medium (learned from Sonarr) |
| Readarr | 52 | Fresh start | Lower (benefited from both) |

## Schema Evolution Patterns

### 1. Initial Schema Design

**Sonarr (Original):**
```sql
-- Core tables
Series (Id, TvdbId, Title, Status, Overview...)
Episodes (Id, SeriesId, SeasonNumber, EpisodeNumber...)
EpisodeFiles (Id, SeriesId, Path, Quality...)
History (Id, EpisodeId, Date, EventType...)
```

**Lidarr (Adapted):**
```sql
-- Core tables with metadata separation
Artists (Id, ArtistMetadataId, Monitored, Path...)
ArtistMetadata (Id, ForeignArtistId, Name, Overview...)
Albums (Id, ArtistMetadataId, Title, ReleaseDate...)
AlbumReleases (Id, AlbumId, ForeignReleaseId...)
Tracks (Id, AlbumReleaseId, TrackNumber, Title...)
TrackFiles (Id, AlbumId, Path, Quality...)
```

**Readarr (Further Refined):**
```sql
-- Enhanced with complex relationships
Authors (Id, AuthorMetadataId, Monitored, Path...)
AuthorMetadata (Id, ForeignAuthorId, Name, Overview...)
Books (Id, AuthorMetadataId, Title, ReleaseDate...)
Editions (Id, BookId, Title, ISBN, Format...)
BookFiles (Id, EditionId, Path, Quality...)
Series (Id, Title, Description...)
SeriesBookLinks (Id, SeriesId, BookId, Position...)
```

### 2. Key Migration Patterns

#### Pattern 1: Metadata Separation
**First seen in Lidarr, adopted by Readarr**

```csharp
// Migration example from Lidarr
public class separate_artist_metadata : NzbDroneMigrationBase
{
    protected override void MainDbUpgrade()
    {
        Create.Table("ArtistMetadata")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("ForeignArtistId").AsString().Unique()
            .WithColumn("Name").AsString()
            .WithColumn("Overview").AsString().Nullable();
            
        Alter.Table("Artists")
            .AddColumn("ArtistMetadataId").AsInt32()
            .ForeignKey("FK_Artists_ArtistMetadata", "ArtistMetadata", "Id");
    }
}
```

**Benefits:**
- Easier metadata updates without affecting core records
- Better handling of metadata provider changes
- Reduced data duplication

#### Pattern 2: Flexible Release/Edition Handling
**Evolved from Album → AlbumRelease → Edition**

```csharp
// Lidarr introduced AlbumReleases
Create.Table("AlbumReleases")
    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
    .WithColumn("AlbumId").AsInt32().ForeignKey()
    .WithColumn("ForeignReleaseId").AsString()
    .WithColumn("Media").AsString() // CD, Vinyl, Digital
    .WithColumn("TrackCount").AsInt32();

// Readarr adapted to Editions
Create.Table("Editions")
    .WithColumn("Id").AsInt32().PrimaryKey().Identity()
    .WithColumn("BookId").AsInt32().ForeignKey()
    .WithColumn("ForeignEditionId").AsString()
    .WithColumn("ISBN13").AsString()
    .WithColumn("Format").AsString() // EPUB, MOBI, PDF
    .WithColumn("Publisher").AsString();
```

#### Pattern 3: Quality Profile Evolution

**Sonarr (Simple):**
```csharp
Create.Table("QualityProfiles")
    .WithColumn("Id").AsInt32().PrimaryKey()
    .WithColumn("Name").AsString()
    .WithColumn("Allowed").AsString() // JSON array of qualities
    .WithColumn("Cutoff").AsInt32();
```

**Lidarr/Readarr (Enhanced):**
```csharp
Create.Table("QualityProfiles")
    .WithColumn("Id").AsInt32().PrimaryKey()
    .WithColumn("Name").AsString()
    .WithColumn("Items").AsString() // JSON with quality items
    .WithColumn("Cutoff").AsInt32()
    .WithColumn("MinFormatScore").AsInt32()
    .WithColumn("CutoffFormatScore").AsInt32();

// Additional metadata profiles
Create.Table("MetadataProfiles")
    .WithColumn("Id").AsInt32().PrimaryKey()
    .WithColumn("Name").AsString()
    .WithColumn("PrimaryTypes").AsString()
    .WithColumn("SecondaryTypes").AsString();
```

### 3. Migration Complexity Analysis

#### Sonarr Migrations (High Complexity)
- Evolved organically over years
- Multiple schema refactorings
- Backward compatibility concerns
- Examples:
  - Migration 070: Delay profiles
  - Migration 094: Add TvMazeId
  - Migration 116: Remove duplicate episodes
  - Migration 147: Custom format additions

#### Lidarr Migrations (Medium Complexity)
- Started fresh with lessons learned
- Cleaner initial schema
- Focus on music-specific needs
- Examples:
  - Migration 001: Initial schema
  - Migration 009: Album releases
  - Migration 023: Add release groups
  - Migration 030: Track file quality

#### Readarr Migrations (Lower Complexity)
- Benefited from both predecessors
- Most refined initial schema
- Book-specific enhancements
- Examples:
  - Migration 001: Initial schema
  - Migration 013: Author metadata
  - Migration 024: Book series support
  - Migration 039: Edition formats

### 4. Common Migration Types

#### Data Structure Migrations
```csharp
// Adding new columns
Alter.Table("Books")
    .AddColumn("ReleaseDate").AsDateTime().Nullable();

// Creating indexes for performance
Create.Index("IX_Books_AuthorMetadataId")
    .OnTable("Books")
    .OnColumn("AuthorMetadataId");

// Adding foreign keys
Alter.Table("Editions")
    .AddColumn("BookId").AsInt32()
    .ForeignKey("FK_Editions_Books", "Books", "Id");
```

#### Data Transformation Migrations
```csharp
// Converting data formats
Execute.Sql(@"
    UPDATE Albums 
    SET ReleaseDate = datetime(ReleaseDate)
    WHERE ReleaseDate IS NOT NULL
");

// Normalizing data
Execute.Sql(@"
    UPDATE Authors 
    SET CleanName = LOWER(REPLACE(Name, ' ', ''))
");
```

#### Performance Optimization Migrations
```csharp
// Adding covering indexes
Create.Index("IX_Books_Monitored_AuthorId")
    .OnTable("Books")
    .OnColumn("Monitored").Ascending()
    .OnColumn("AuthorMetadataId").Ascending()
    .WithOptions().NonClustered();

// Denormalization for performance
Alter.Table("Authors")
    .AddColumn("BookCount").AsInt32().WithDefaultValue(0);
```

### 5. Migration Best Practices Observed

#### 1. Incremental Changes
Each migration makes small, focused changes rather than large refactorings.

#### 2. Data Preservation
Migrations include data transformation logic to preserve existing data:
```csharp
// Example from Readarr
Execute.WithConnection((conn, tran) =>
{
    var rows = conn.Query<dynamic>("SELECT * FROM Books");
    foreach (var row in rows)
    {
        // Transform and preserve data
    }
});
```

#### 3. Rollback Safety
Later projects include more rollback considerations:
```csharp
public override void Down()
{
    Delete.Column("NewColumn").FromTable("TableName");
}
```

#### 4. Performance Considerations
Indexes are added strategically:
- After bulk data operations
- On foreign key columns
- For common query patterns

### 6. Schema Design Evolution

#### Normalization Progression

**Sonarr**: 2nd Normal Form
- Some denormalization for performance
- Direct relationships

**Lidarr**: 3rd Normal Form
- Metadata separation
- Proper foreign keys

**Readarr**: 3rd Normal Form+
- Full metadata separation
- Junction tables for many-to-many
- Careful denormalization

#### Relationship Complexity

**Sonarr**:
```
Series 1:N Episodes
Episodes 1:1 EpisodeFile
```

**Lidarr**:
```
Artist 1:N Albums
Album 1:N AlbumReleases
AlbumRelease 1:N Tracks
Track N:1 TrackFile
```

**Readarr**:
```
Author 1:N Books
Book 1:N Editions
Edition 1:N BookFiles
Book N:N Series (via SeriesBookLinks)
```

### 7. Migration Strategy Recommendations

For new media types, follow these database migration principles:

1. **Start with normalized schema** - Easier to denormalize later
2. **Separate volatile metadata** - External data in separate tables
3. **Plan for relationships** - Many-to-many from the start
4. **Index strategically** - Add indexes based on query patterns
5. **Version everything** - Include schema version in database
6. **Test migrations** - Always test up and down migrations
7. **Preserve data** - Never lose user data during migrations

### 8. Future Database Considerations

#### Potential Improvements
1. **Partition large tables** - History, Log tables by date
2. **Archive old data** - Move old history to archive tables
3. **Optimize queries** - Materialized views for complex queries
4. **Consider NoSQL** - For metadata/cache storage
5. **Implement sharding** - For very large libraries

#### Schema Versioning
Consider implementing a more robust schema versioning system:
```csharp
Create.Table("SchemaInfo")
    .WithColumn("Version").AsInt32().PrimaryKey()
    .WithColumn("AppliedOn").AsDateTime()
    .WithColumn("Description").AsString();
```

## Conclusion

The database migration analysis reveals:

1. **Progressive refinement** - Each project learned from its predecessor
2. **Increasing sophistication** - Schema design improved with each iteration
3. **Domain-driven design** - Schema reflects domain complexity accurately
4. **Performance awareness** - Later projects show better optimization
5. **Maintainability focus** - Cleaner migrations in newer projects

This evolution provides a solid foundation for future media type adaptations, with established patterns for handling complex relationships, metadata management, and performance optimization.