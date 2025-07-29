# Phase 2: API Layer Transformation

## Overview
This phase transforms the API layer from TV-focused endpoints to book-focused endpoints, maintaining compatibility with the *arr API patterns while adapting to book domain concepts.

## API Endpoint Mapping

### Primary Resource Endpoints

| TV API (v3) | Book API (v1) | Description |
|-------------|---------------|-------------|
| `/api/v3/series` | `/api/v1/author` | Primary content creator |
| `/api/v3/series/{id}` | `/api/v1/author/{id}` | Get specific author |
| `/api/v3/series/lookup` | `/api/v1/author/lookup` | Search for authors |
| `/api/v3/episode` | `/api/v1/edition` | Individual items |
| `/api/v3/episode/{id}` | `/api/v1/edition/{id}` | Get specific edition |
| `/api/v3/episodefile` | `/api/v1/bookfile` | File management |
| - | `/api/v1/book` | Book management (new) |
| - | `/api/v1/series` | Book series (new) |

### Resource Transformations

#### SeriesResource → AuthorResource
```csharp
// Old
public class SeriesResource
{
    public int Id { get; set; }
    public int TvdbId { get; set; }
    public string Title { get; set; }
    public string Overview { get; set; }
    public string Network { get; set; }
    public string AirTime { get; set; }
    // ...
}

// New
public class AuthorResource
{
    public int Id { get; set; }
    public string ForeignAuthorId { get; set; }
    public string Name { get; set; }
    public string Overview { get; set; }
    public string Website { get; set; }
    public AuthorMetadataResource Metadata { get; set; }
    // ...
}
```

#### EpisodeResource → EditionResource
```csharp
// Old
public class EpisodeResource
{
    public int Id { get; set; }
    public int SeriesId { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Title { get; set; }
    public DateTime? AirDate { get; set; }
    // ...
}

// New
public class EditionResource
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string ForeignEditionId { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public string Language { get; set; }
    public string Format { get; set; }
    public DateTime? ReleaseDate { get; set; }
    // ...
}
```

### New Resources

#### BookResource
```csharp
public class BookResource
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string ForeignBookId { get; set; }
    public string Title { get; set; }
    public string Isbn { get; set; }
    public BookMetadataResource Metadata { get; set; }
    public List<EditionResource> Editions { get; set; }
    // ...
}
```

#### SeriesResource (Book Series)
```csharp
public class SeriesResource
{
    public int Id { get; set; }
    public string ForeignSeriesId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int AuthorId { get; set; }
    public List<SeriesBookResource> Books { get; set; }
    // ...
}
```

## Controller Structure

### AuthorController (from SeriesController)
- `GET /api/v1/author` - Get all authors
- `GET /api/v1/author/{id}` - Get specific author
- `POST /api/v1/author` - Add new author
- `PUT /api/v1/author/{id}` - Update author
- `DELETE /api/v1/author/{id}` - Delete author
- `GET /api/v1/author/lookup` - Search for authors

### BookController (new)
- `GET /api/v1/book` - Get all books
- `GET /api/v1/book/{id}` - Get specific book
- `GET /api/v1/book/author/{authorId}` - Get books by author
- `POST /api/v1/book` - Add new book
- `PUT /api/v1/book/{id}` - Update book
- `DELETE /api/v1/book/{id}` - Delete book

### EditionController (from EpisodeController)
- `GET /api/v1/edition` - Get all editions
- `GET /api/v1/edition/{id}` - Get specific edition
- `GET /api/v1/edition/book/{bookId}` - Get editions by book
- `PUT /api/v1/edition/{id}` - Update edition
- `PUT /api/v1/edition/monitor` - Bulk monitor/unmonitor

### BookFileController (from EpisodeFileController)
- `GET /api/v1/bookfile` - Get all book files
- `GET /api/v1/bookfile/{id}` - Get specific book file
- `DELETE /api/v1/bookfile/{id}` - Delete book file
- `PUT /api/v1/bookfile/editor` - Bulk edit book files

### SeriesController (Book Series - new)
- `GET /api/v1/series` - Get all book series
- `GET /api/v1/series/{id}` - Get specific series
- `GET /api/v1/series/author/{authorId}` - Get series by author
- `POST /api/v1/series` - Add new series
- `PUT /api/v1/series/{id}` - Update series

## Mapping Profiles

### AuthorResourceMapper
- Maps between Author domain model and AuthorResource
- Includes metadata mapping
- Handles book count statistics

### BookResourceMapper
- Maps between Book domain model and BookResource
- Includes edition mapping
- Handles series relationships

### EditionResourceMapper
- Maps between Edition domain model and EditionResource
- Includes file information
- Handles format/language details

## API Versioning Strategy
- Keep v1 API for Readarr (book-focused)
- Remove v3 API (TV-focused)
- Remove v5 API if present
- Maintain backward compatibility where possible

## Implementation Steps
1. Create new resource classes in Readarr.Api.V1
2. Transform existing controllers to book domain
3. Create new controllers for book-specific features
4. Update resource mappers
5. Update Swagger documentation
6. Update API tests