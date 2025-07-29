# ADR-001: Direct Fork from Sonarr to Readarr v2

## Status
Accepted

## Context
The goal is to create Readarr v2 by transforming the existing Sonarr codebase. This decision record outlines the initial architectural approach for this transformation.

## Decision
We will fork directly from Sonarr to create Readarr v2, implementing the following key architectural decisions from the start:

1. **Metadata Separation Pattern**: Implement clean separation between core media functionality and book-specific metadata from the beginning. This means:
   - Core entities (e.g., `MediaFile`, `QualityProfile`) will remain generic
   - Book-specific entities (e.g., `Author`, `Book`, `Edition`) will reside in a dedicated `NzbDrone.Core.Books` namespace/project
   - Metadata providers (e.g., TVDB, TMDB) will be replaced with book-specific providers (e.g., Goodreads, Open Library)

2. **API Versioning**: The API will be versioned as `v1` from the start, allowing for future iterations without breaking changes

3. **Frontend Framework**: Continue using React for the frontend, adapting existing components and creating new ones as needed for the book domain

## Consequences
- **Pros**:
  - Leverages a mature and stable codebase (Sonarr)
  - Reduces initial development time compared to building from scratch
  - Provides a clear path for future feature parity with Sonarr where applicable
  - Clean metadata separation will make it easier to manage book-specific logic and potentially integrate other media types in the future (though not a primary goal for v2)
- **Cons**:
  - Requires significant refactoring and renaming across the entire codebase
  - Potential for lingering TV-centric concepts if not thoroughly transformed
  - Initial build and test failures are expected during the transformation process