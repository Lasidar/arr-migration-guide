# Phase 1: Domain Model Transformation Mapping

## Overview
This document outlines the transformation from Sonarr's TV-focused domain model to Readarr's book-focused domain model.

## Entity Mapping

### Primary Entities
| Sonarr | Readarr | Purpose |
|--------|---------|---------|
| Series | Author | Primary content creator/container |
| Season | Book | Primary content collection |
| Episode | Edition | Individual consumable unit |

### Additional Entities
| Sonarr | Readarr | Purpose |
|--------|---------|---------|
| - | Series | Book series (new concept) |
| - | AuthorMetadata | Separated metadata (following Lidarr pattern) |
| - | BookMetadata | Separated metadata for books |

## Class Transformation Plan

### 1. Series → Author
**Files to transform:**
- `src/Readarr.Core/Tv/Series.cs` → `src/Readarr.Core/Books/Author.cs`
- `src/Readarr.Core/Tv/SeriesRepository.cs` → `src/Readarr.Core/Books/AuthorRepository.cs`
- `src/Readarr.Core/Tv/SeriesService.cs` → `src/Readarr.Core/Books/AuthorService.cs`

**Key changes:**
- Add `AuthorMetadataId` property
- Replace TV-specific properties (Network, AirTime) with book-specific ones
- Add lazy loading for Books and Series collections
- Change base class from `ModelBase` to `Entity<Author>`

### 2. Season → Book
**Files to transform:**
- `src/Readarr.Core/Tv/Season.cs` → `src/Readarr.Core/Books/Book.cs`

**Key changes:**
- Add `BookMetadataId` property
- Add ISBN, ASIN, GoodreadsId properties
- Add PageCount, ReleaseDate
- Implement lazy loading for Editions

### 3. Episode → Edition
**Files to transform:**
- `src/Readarr.Core/Tv/Episode.cs` → `src/Readarr.Core/Books/Edition.cs`
- `src/Readarr.Core/Tv/EpisodeRepository.cs` → `src/Readarr.Core/Books/EditionRepository.cs`
- `src/Readarr.Core/Tv/EpisodeService.cs` → `src/Readarr.Core/Books/EditionService.cs`

**Key changes:**
- Replace episode-specific properties with edition-specific ones
- Add ISBN, ASIN for edition-specific identifiers
- Add Publisher, Language, Format properties

### 4. New Entity: Series (Book Series)
**Files to create:**
- `src/Readarr.Core/Books/Series.cs`
- `src/Readarr.Core/Books/SeriesRepository.cs`
- `src/Readarr.Core/Books/SeriesService.cs`

**Properties:**
- Id, Title, Description
- AuthorId (foreign key)
- Books collection
- Position tracking for books in series

## Namespace Changes
- `Readarr.Core.Tv` → `Readarr.Core.Books`
- `Readarr.Core.Tv.Commands` → `Readarr.Core.Books.Commands`
- `Readarr.Core.Tv.Events` → `Readarr.Core.Books.Events`

## Database Schema Changes
### Tables to Rename
- Series → Authors
- Seasons → Books  
- Episodes → Editions
- SeriesStatistics → AuthorStatistics

### New Tables
- AuthorMetadata
- BookMetadata
- Series (for book series)
- SeriesBooks (junction table)

### Column Mappings
**Series → Authors:**
- TvdbId → GoodreadsId
- Title → Name
- CleanTitle → CleanName
- Network → (remove)
- AirTime → (remove)
- Add: AuthorMetadataId

**Seasons → Books:**
- SeriesId → AuthorId
- SeasonNumber → (remove)
- Add: BookMetadataId, ISBN, ASIN, PageCount

**Episodes → Editions:**
- SeasonId → BookId
- EpisodeNumber → (remove)
- AirDate → ReleaseDate
- Add: ISBN, Publisher, Language, Format

## Service Layer Changes
### Command Updates
- RefreshSeriesCommand → RefreshAuthorCommand
- MoveSeriesCommand → MoveAuthorCommand
- BulkMoveSeriesCommand → BulkMoveAuthorCommand

### Event Updates
- SeriesAddedEvent → AuthorAddedEvent
- SeriesDeletedEvent → AuthorDeletedEvent
- SeriesUpdatedEvent → AuthorUpdatedEvent
- EpisodeInfoRefreshedEvent → EditionInfoRefreshedEvent

## Repository Pattern Updates
All repositories need to:
1. Update entity types
2. Update query methods
3. Update relationship mappings
4. Add new query methods for book-specific needs

## Next Steps
1. Create base Author, Book, Edition classes
2. Create metadata entities
3. Update repositories
4. Update services
5. Create database migrations
6. Update API controllers
7. Update UI components