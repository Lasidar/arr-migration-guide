# Phase 4: Metadata Provider Integration

## Overview
Phase 4 focuses on integrating external metadata providers to fetch book and author information. This replaces TV metadata providers (like TheTVDB) with book-focused providers (like Goodreads/BookInfo).

## Tasks

### Task 4.1: Create Metadata Provider Documentation
- Document provider interfaces
- Define data flow
- Establish mapping patterns

### Task 4.2: Implement Goodreads/BookInfo Provider
- Create BookInfoProxy service
- Implement HTTP client integration
- Handle API authentication and rate limiting

### Task 4.3: Create Search Criteria Models
- AuthorSearchCriteria
- BookSearchCriteria
- ISBN search support
- Title/Author fuzzy matching

### Task 4.4: Implement Author Search
- Search by name
- Search by Goodreads ID
- Handle multiple results
- Disambiguation logic

### Task 4.5: Implement Book Search
- Search by title
- Search by ISBN
- Search by author
- Combined search capabilities

### Task 4.6: Implement Metadata Mapping
- Map external API responses to domain models
- Handle missing/incomplete data
- Image URL mapping
- Rating conversions

### Task 4.7: Update Indexer Integration
- Modify search parameters for books
- Add ISBN support to indexers
- Update category mappings
- Book-specific quality profiles

### Task 4.8: Commit Phase 4 Changes
- Test metadata providers
- Documentation updates
- Git commit and push

## Provider Architecture

### External API Integration
```
External API (Goodreads/BookInfo)
    ↓
HTTP Client with Rate Limiting
    ↓
Response Parsing & Validation
    ↓
Domain Model Mapping
    ↓
Local Database Cache
```

### Search Flow
1. User searches for author/book
2. Search criteria built
3. External API queried
4. Results mapped to domain models
5. Duplicates filtered
6. Results presented to user

## Key Interfaces

### IProvideAuthorInfo
- GetAuthorInfo(foreignAuthorId)
- SearchForNewAuthor(title)
- GetAuthorMetadata(foreignAuthorId)

### IProvideBookInfo  
- GetBookInfo(foreignBookId)
- GetBooksForAuthor(authorId)
- SearchForNewBook(criteria)

### ISearchForNewEntity
- Generic search interface
- Supports multiple providers
- Result aggregation

## Data Mapping

### Author Mapping
- Goodreads Author ID → ForeignAuthorId
- Name variations → Aliases
- Biography → Overview
- Author images → MediaCover

### Book Mapping
- Goodreads Book ID → ForeignBookId
- ISBN/ISBN13 → Identifiers
- Publication info → Metadata
- Cover images → MediaCover
- Ratings → Rating model

## Error Handling
- API timeouts
- Rate limiting (429 responses)
- Invalid/missing data
- Network failures
- Authentication errors