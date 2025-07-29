# Migration Analysis Quick Start Guide

## Overview

This guide provides immediate actionable steps to begin analyzing the Sonarr → Lidarr → Readarr migration patterns. Use this alongside the comprehensive migration-analysis-plan.md.

## Prerequisites

- Git installed and configured
- Development environment capable of running .NET applications
- Basic understanding of media management systems
- Access to the repository with submodules

## Immediate Analysis Steps

### Step 1: Verify Fork Points (30 minutes)

```bash
# Check Sonarr at the Lidarr fork point
cd sonarr
git checkout 83370dd
git log --oneline -10

# Check Lidarr at the Readarr fork point  
cd ../lidarr
git checkout 47f4441
git log --oneline -10
```

Document:
- The exact date and context of each fork
- The last few commits before forking
- Any preparation commits for the fork

### Step 2: Quick Architecture Survey (1 hour)

Run these commands in each project directory:

```bash
# Get project structure
find . -type f -name "*.csproj" | head -20

# Identify main entry points
find . -name "Program.cs" -o -name "Startup.cs" | head -10

# Look for domain models
find . -type d -name "Models" -o -name "Entities" | head -10

# Find API controllers
find . -name "*Controller.cs" | head -20
```

### Step 3: Domain Model Quick Comparison (2 hours)

Key files to examine first:

**Sonarr:**
- Look for: Series.cs, Season.cs, Episode.cs
- Database models in: src/NzbDrone.Core/Tv/
- API models in: src/Sonarr.Api.*/

**Lidarr:**
- Look for: Artist.cs, Album.cs, Track.cs
- Database models in: src/NzbDrone.Core/Music/
- API models in: src/Lidarr.Api.*/

**Readarr:**
- Look for: Author.cs, Book.cs, Edition.cs
- Database models in: src/NzbDrone.Core/Books/
- API models in: src/Readarr.Api.*/

### Step 4: Database Schema Extraction (1 hour)

```bash
# Find migration files
find . -name "*Migration*.cs" | grep -i db

# Look for schema definitions
grep -r "CREATE TABLE" --include="*.cs" --include="*.sql"

# Find entity configurations
find . -name "*Configuration.cs" | grep -i entity
```

### Step 5: Integration Points Survey (1 hour)

Search for external service integrations:

```bash
# Metadata providers
grep -r "metadata" --include="*.cs" | grep -i provider

# Download clients  
grep -r "download.*client" --include="*.cs" -i

# Indexers
find . -name "*Indexer*.cs"

# Notifications
find . -name "*Notification*.cs"
```

## Key Comparison Areas

### 1. Namespace Changes
- Sonarr: `NzbDrone.Core.Tv`
- Lidarr: `NzbDrone.Core.Music`  
- Readarr: `NzbDrone.Core.Books`

### 2. Core Entity Mappings

| Sonarr | Lidarr | Readarr |
|--------|--------|---------|
| Series | Artist | Author |
| Season | Album | Book |
| Episode | Track | Edition |
| SeriesType | ArtistType | BookType |
| SeriesStatistics | ArtistStatistics | AuthorStatistics |

### 3. Metadata Providers

| Type | Sonarr | Lidarr | Readarr |
|------|--------|--------|---------|
| Primary | TheTVDB | MusicBrainz | Goodreads |
| Secondary | TVMaze | Last.fm | OpenLibrary |
| Tertiary | TMDB | Discogs | GoogleBooks |

### 4. Quality Profiles

Look for media-specific quality definitions:
- Sonarr: HDTV, WebDL, Bluray
- Lidarr: MP3, FLAC, AAC
- Readarr: EPUB, MOBI, PDF

## Quick Wins for Analysis

1. **Generate Class Diagrams**: Use tools like PlantUML to visualize entity relationships
2. **API Endpoint Comparison**: Export and diff API route tables
3. **Configuration Diff**: Compare appsettings.json and config files
4. **Test Coverage Report**: Run test coverage to identify well-tested migration areas

## Common Patterns to Look For

1. **Interface Abstraction**: How media-specific logic is abstracted
2. **Factory Patterns**: How different media types are instantiated
3. **Strategy Patterns**: How different providers are selected
4. **Repository Patterns**: How data access is abstracted
5. **Event Patterns**: How media-specific events are handled

## Tools for Analysis

- **Visual Studio/Rider**: For code navigation and refactoring analysis
- **Git Diff Tools**: For comparing large code changes
- **Database Tools**: For schema comparison
- **API Testing Tools**: Postman/Insomnia for API exploration
- **Dependency Analyzers**: To understand architectural dependencies

## Documentation Templates

Create these files as you analyze:

```
docs/analysis/
├── entity-mapping.md
├── api-changes.md
├── database-schema-evolution.md
├── integration-changes.md
├── configuration-changes.md
└── ui-terminology-mapping.md
```

## Red Flags to Watch For

1. **Hardcoded Media Types**: Look for places where media type wasn't properly abstracted
2. **Incomplete Migrations**: Features that exist in parent but not in fork
3. **Technical Debt**: Comments indicating temporary solutions
4. **Breaking Changes**: API or database changes that aren't backward compatible

## Next Steps After Quick Start

1. Deep dive into the most changed files
2. Set up development environments to run the applications
3. Create automated comparison scripts
4. Begin documenting patterns in the provided templates
5. Schedule review of initial findings

Remember: Focus on patterns and architecture, not line-by-line code changes!