# Commit Migration Guidelines: Sonarr to Readarr

Since Sonarr is the upstream ancestor of both Lidarr and Readarr, it's often beneficial to migrate fixes and improvements from Sonarr to Readarr. However, since Readarr is not a direct fork of Sonarr (it's forked from Lidarr), this process requires careful adaptation. Below are guidelines for effectively migrating commits from Sonarr to Readarr.

## Domain Terminology Adaptations

When migrating commits, be aware of these key domain terminology differences:

| Sonarr (TV) | Readarr (Books) |
|-------------|----------------|
| Series | Author |
| Season | Book |
| Episode | Edition/Format |
| ShowTitle | BookTitle |
| EpisodeFile | BookFile |
| TvdbId | GoodreadsId/GoogleBooksId/IsbnId |
| SeriesType | BookType |
| AirDate | ReleaseDate |
| Network | Publisher |
| Runtime | PageCount |

**Tips for Terminology Migration:**
- Use search and replace with caution - ensure you're not changing unrelated terms
- Watch for camelCase vs PascalCase variations (e.g., `seriesId` vs `authorId`)
- Remember to update variable names, class names, table names, and UI strings
- Check for plural forms as well (e.g., `Series` might be both singular and plural, but `Authors` vs `Author`)

## Domain Logic Adaptations

When adapting domain logic:

1. **Media Structure Differences**:
   - TV has a Series → Season → Episode hierarchy
   - Books have an Author → Book → Edition hierarchy

2. **Scheduling Logic**:
   - TV episodes are scheduled by air date
   - Books are scheduled by release date and may have multiple editions

3. **Quality and Format Handling**:
   - For TV, quality typically means resolution/source
   - For books, formats include different media (PDF, EPUB, MOBI, Physical, Audiobook)

4. **Monitoring Settings**:
   - Sonarr allows monitoring at series and season level
   - Readarr focuses on book level monitoring

## UI Component Migration

When migrating UI changes:

1. **Page Structure**:
   - Verify if the page structure is similar between Sonarr and Readarr
   - Check if the component exists in the same location in the component hierarchy

2. **Component Properties**:
   - Adapt property names to match Readarr's domain terminology
   - Watch for dependent properties that might also need renaming

3. **UI Text**:
   - Update all visible strings to use book terminology
   - Check tooltips, error messages, and other hidden text

## Metadata Source Considerations

Sonarr and Readarr use different metadata providers:

1. **API Differences**:
   - Sonarr: TheTVDB, TMDB (structured TV data)
   - Readarr: Goodreads, Google Books (book-specific metadata)

2. **Metadata Fields**:
   - Watch for TV-specific metadata fields that need mapping to book equivalents
   - Some fields may not have direct equivalents (e.g., broadcast schedule)

3. **Search Parameters**:
   - Search algorithms differ between TV shows and books
   - Book searches may rely on ISBN, ASIN, or similar identifiers

## Code Sections Typically Not Relevant

Some Sonarr code sections may be irrelevant to Readarr:

1. **TV-Specific Features**:
   - Season pass handling
   - Episode scene numbering
   - TV networks integration

2. **External Integrations**:
   - TV-specific Plex/Kodi/Emby mappings
   - TV calendar integrations

3. **Certain Parser Logic**:
   - Scene naming conventions specific to TV releases

## Migration Process Best Practices

1. **Analyze the Original Commit**:
   - Understand what problem it solves before attempting migration
   - Identify if the issue applies to Readarr's domain

2. **Search for Similar Code in Readarr**:
   - Look for the equivalent functionality in Readarr
   - Check if Lidarr has already adapted this code (might be easier to port from there)

3. **Test Thoroughly**:
   - Create specific test cases for the book domain
   - Verify edge cases specific to book metadata

4. **Documentation**:
   - Reference the original Sonarr commit in your PR
   - Note any adaptations made for the book domain

By following these guidelines, developers can more effectively migrate valuable improvements from Sonarr to Readarr, maintaining the benefits of the shared architecture while respecting the differences in domain models.

## Navigating the Indirect Fork Relationship

Since Readarr was forked from Lidarr (not directly from Sonarr), additional considerations come into play:

1. **Check Lidarr's Implementation First**:
   - Before directly porting from Sonarr to Readarr, check if Lidarr has already adapted the code
   - Lidarr's adaptation might be closer to what Readarr needs (Music domain → Book domain vs. TV domain → Book domain)

2. **Watch for Lidarr-Specific Abstractions**:
   - Lidarr may have created intermediate abstractions not present in Sonarr
   - These abstractions might already accommodate multiple media types

3. **Consider the Migration Path**:
   ```
   Sonarr → Lidarr → Readarr
   ```
   - Some concepts might be renamed twice along the path:
     - `Series` (Sonarr) → `Artist` (Lidarr) → `Author` (Readarr)
     - `Episode` (Sonarr) → `Track` (Lidarr) → `Edition` (Readarr)

4. **Handling Conflicting Changes**:
   - If both Lidarr and Readarr have diverged from a common code point, reconcile these differences
   - Look for places where Readarr intentionally deviates from Lidarr's pattern

5. **Version Disparities**:
   - Be aware that Sonarr, Lidarr, and Readarr may be at different points in their development cycles
   - Some features in newer Sonarr versions might not exist in Lidarr/Readarr yet

## Example Migration Workflow

1. **Identify valuable Sonarr commit** that should be migrated to Readarr
2. **Check if Lidarr has already incorporated** this change (may save significant effort)
3. **Determine the fundamental problem** being solved by the commit
4. **Locate the equivalent code section** in Readarr
5. **Adapt the solution** using the terminology and architecture specific to Readarr
6. **Test thoroughly** in the book domain context
7. **Document the adaptation** and reference original commits from both Sonarr and Lidarr (if applicable)

With this approach, developers can effectively bridge the gap between Sonarr and Readarr despite the indirect fork relationship, ensuring that improvements propagate throughout the *arr ecosystem.

## Common Code Patterns Requiring Adaptation

Below are specific examples of code patterns that typically need adaptation when migrating from Sonarr to Readarr:

### 1. Domain Class References

```csharp
// Sonarr
public void ProcessSeries(Series series)
{
    foreach (var season in series.Seasons)
    {
        foreach (var episode in season.Episodes)
        {
            // Process episode
        }
    }
}

// Readarr
public void ProcessAuthor(Author author)
{
    foreach (var book in author.Books)
    {
        foreach (var edition in book.Editions)
        {
            // Process edition
        }
    }
}
```

### 2. Database Entity References

```csharp
// Sonarr
var episodeFile = _episodeFileRepository.Find(episodeFileId);
episodeFile.Quality = newQuality;
_episodeFileRepository.Update(episodeFile);

// Readarr
var bookFile = _bookFileRepository.Find(bookFileId);
bookFile.Quality = newQuality;
_bookFileRepository.Update(bookFile);
```

### 3. API Endpoints

```csharp
// Sonarr
[RestResource(Name = "Series")]
public class SeriesController : RestController<SeriesResource>
{
    // Series endpoints
}

// Readarr
[RestResource(Name = "Author")]
public class AuthorController : RestController<AuthorResource>
{
    // Author endpoints
}
```

### 4. Frontend Component Adaptations

```javascript
// Sonarr (React component)
class SeriesDetails extends Component {
  render() {
    const { series, seasons } = this.props;
    
    return (
      <div>
        <h1>{series.title}</h1>
        <div>Network: {series.network}</div>
        {/* seasons list */}
      </div>
    );
  }
}

// Readarr adaptation
class AuthorDetails extends Component {
  render() {
    const { author, books } = this.props;
    
    return (
      <div>
        <h1>{author.name}</h1>
        <div>Publisher: {author.publisher}</div>
        {/* books list */}
      </div>
    );
  }
}
```

### 5. Migration of Settings

```csharp
// Sonarr
public class SeriesSettings
{
    public bool AutoUnmonitorPreviouslyAiredEpisodes { get; set; }
    public bool AutomaticallySendTorrentDownloadsToSickGear { get; set; }
}

// Readarr
public class AuthorSettings
{
    public bool AutoUnmonitorPreviouslyReleasedBooks { get; set; }
    public bool AutomaticallySendTorrentDownloadsToCalibri { get; set; }
}
```

### 6. API Response Structure

```json
// Sonarr API response
{
  "series": {
    "id": 1,
    "title": "Breaking Bad",
    "tvdbId": 81189,
    "seasons": [
      {
        "seasonNumber": 1,
        "monitored": true
      }
    ]
  }
}

// Readarr equivalent
{
  "author": {
    "id": 1,
    "name": "Stephen King",
    "goodreadsId": 3389,
    "books": [
      {
        "title": "The Shining",
        "monitored": true
      }
    ]
  }
}
```

### 7. File Structure and Naming Conventions

```csharp
// Sonarr
public string BuildFilePath(Series series, Episode episode)
{
    return Path.Combine(
        _rootPath,
        FileNameBuilder.BuildFileName(series, episode)
    );
}

// Readarr
public string BuildFilePath(Author author, Book book)
{
    return Path.Combine(
        _rootPath,
        FileNameBuilder.BuildFileName(author, book)
    );
}
```

By recognizing these common patterns, developers can more systematically approach the migration process, ensuring consistent adaptations across the codebase.