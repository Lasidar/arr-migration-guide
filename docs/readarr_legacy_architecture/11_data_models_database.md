# Data Models and Database

This document details the data model structure and entity relationships in Readarr's database.

## Database Overview

Readarr uses SQLite by default for data storage, with optional support for PostgreSQL. The database schema is managed through a migration framework that incrementally updates the schema as new versions of the application are released.

## Entity Relationship Diagram

```mermaid
erDiagram
    Authors ||--o{ Books : has
    AuthorMetadata ||--o{ Authors : referenced_by
    AuthorMetadata ||--o{ Books : referenced_by
    Books ||--o{ Editions : has
    Editions ||--o{ BookFiles : has
    Series ||--o{ SeriesBookLink : has
    Books ||--o{ SeriesBookLink : has
    Authors }|--|| QualityProfiles : uses
    Authors }|--|| MetadataProfiles : uses
    BookFiles }|--|| QualityDefinitions : has
    Tags ||--o{ Authors : tagged_with
    
    Authors {
        int Id PK
        int AuthorMetadataId FK
        string Path
        bool Monitored
        int QualityProfileId FK
        int MetadataProfileId FK
        dateTime Added
        dateTime LastInfoSync
        string[] Tags
        NewItemMonitorTypes MonitorNewItems
    }
    
    AuthorMetadata {
        int Id PK
        string ForeignAuthorId
        string Name
        string SortName
        string NameLastFirst
        string SortNameLastFirst
        string Aliases
        string Overview
        string Gender
        string Hometown
        string Born
        string Died
        string Status
        string[] Images
        string[] Links
        Ratings Ratings
        string[] Genres
    }
    
    Books {
        int Id PK
        int AuthorMetadataId FK
        string ForeignBookId
        string Title
        string TitleSlug
        dateTime ReleaseDate
        string[] Links
        string[] Genres
        Ratings Ratings
        bool Monitored
        bool AnyEditionOk
        dateTime LastInfoSync
        dateTime Added
    }
    
    Editions {
        int Id PK
        int BookId FK
        string ForeignEditionId
        string Isbn13
        string Asin
        string Title
        string TitleSlug
        string Language
        string Overview
        string Format
        bool IsEbook
        string Publisher
        int PageCount
        dateTime ReleaseDate
        string[] Images
        string[] Links
        Ratings Ratings
        bool Monitored
    }
    
    BookFiles {
        int Id PK
        int EditionId FK
        int CalibreId
        string Path
        long Size
        QualityModel Quality
        dateTime DateAdded
        string ReleaseGroup
        string SceneName
    }
    
    Series {
        int Id PK
        string ForeignSeriesId
        string Title
        string Description
        bool Numbered
    }
    
    SeriesBookLink {
        int Id PK
        int SeriesId FK
        int BookId FK
        string Position
        int SeriesPosition
        bool IsPrimary
    }
    
    QualityProfiles {
        int Id PK
        string Name
        int Cutoff FK
        bool UpgradeAllowed
        QualityItems[] Items
    }
    
    MetadataProfiles {
        int Id PK
        string Name
        int MinPopularity
        bool SkipMissingDate
        bool SkipMissingIsbn
        bool SkipPartsAndSets
        bool SkipSeriesSecondary
        string[] AllowedLanguages
    }
    
    QualityDefinitions {
        int Id PK
        int Quality
        string Title
        int Weight
        int MinSize
        int MaxSize
    }
    
    Tags {
        int Id PK
        string Label
    }
    
    EntityHistory {
        int Id PK
        int BookId FK
        int AuthorId FK
        string SourceTitle
        QualityModel Quality
        dateTime Date
        EntityHistoryEventType EventType
        Dictionary Data
        string DownloadId
    }
```

## Core Entities

### Author
Authors are central to Readarr's data model. An Author represents a writer whose work is being tracked. The Author entity is primarily for tracking and management purposes, while actual metadata about the author is stored in AuthorMetadata.

Key relationships:
- Links to AuthorMetadata for author details
- Has many Books
- Uses QualityProfile for determining acceptable file quality
- Uses MetadataProfile for determining metadata requirements
- Can have Tags

### AuthorMetadata
Contains the core metadata for an author, which can be shared across multiple Author entities.

Key attributes:
- Name, SortName
- Biography/Overview
- Gender, Hometown
- Birth/Death information
- Status (continuing, ended, etc.)
- Images, Links, Ratings, Genres

### Book
Represents a book title in the library. Books are associated with an author through the AuthorMetadata relationship.

Key relationships:
- Belongs to an AuthorMetadata
- Has many Editions
- Can be part of Series through SeriesBookLink

### Edition
Editions represent specific releases of a book. A Book can have multiple Editions (e.g., hardcover, paperback, ebook).

Key attributes:
- ISBN13, ASIN identifiers
- Language
- Format and IsEbook flag
- Publisher information
- Page count
- Release date

Key relationships:
- Belongs to a Book
- Has many BookFiles

### BookFile
Represents the actual files on disk for a specific Edition.

Key attributes:
- File path
- Size
- Quality information
- Date added
- Release group information

Key relationships:
- Belongs to an Edition

### Series
Represents a book series (e.g., "Harry Potter", "The Lord of the Rings").

Key attributes:
- Title
- Description
- Whether it's numbered

Key relationships:
- Connected to Books through SeriesBookLink

### SeriesBookLink
Junction table that connects Books to Series, storing the position of a book within a series.

Key attributes:
- Position (string position like "1.5" or "Book 2")
- SeriesPosition (integer position for sorting)
- IsPrimary (whether this is the main series for the book)

## Configuration Entities

### QualityProfile
Defines quality preferences for downloading/upgrading books.

Key attributes:
- Name
- Cutoff (minimum acceptable quality)
- Whether upgrades are allowed
- Items (list of quality options and their allowed status)

### MetadataProfile
Defines metadata requirements and filtering for importing books.

Key attributes:
- Name
- MinPopularity
- Whether to skip books missing date, ISBN, etc.
- Whether to skip parts/sets
- Allowed languages

### QualityDefinition
Defines quality types, their weights, and size constraints.

Key attributes:
- Quality ID
- Title
- Weight (for sorting quality levels)
- Min/Max Size limits

## Tracking Entities

### EntityHistory
Tracks events related to books such as downloads, imports, and deletions.

Key attributes:
- BookId and AuthorId
- SourceTitle (where the book came from)
- Quality
- Date
- EventType (Grabbed, BookFileImported, etc.)
- Additional Data dictionary

### Tag
Used for organizing and filtering authors.

Key attributes:
- Label

## Data Storage

### JSON Embedding
Readarr makes use of embedded JSON for complex data types:
- Images, Links, and Genres are stored as JSON arrays
- Ratings is stored as a JSON object
- Quality is stored as a JSON object with nested Revision
- Additional data in EntityHistory is stored as a JSON dictionary

### Lazy Loading
Entity relationships are loaded on-demand using LazyLoaded<T> wrappers to improve performance.

## Database Operations

### Repositories
The data access layer is implemented using the Repository pattern, with a BasicRepository<T> base class providing common operations.

Key repositories include:
- AuthorRepository
- BookRepository
- EditionRepository
- BookFileRepository
- SeriesRepository
- QualityProfileRepository

### ORM
Readarr uses a custom lightweight ORM with the following features:
- Table mapping through attributes and conventions
- Builder pattern for query construction
- Support for both SQLite and PostgreSQL

### Migrations
Database schema changes are managed through numbered migrations that are applied sequentially at application startup. Each migration contains the SQL statements needed to update the schema from one version to the next. 