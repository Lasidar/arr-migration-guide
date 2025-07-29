# Phase 5: Download & Import Pipeline

## Overview
Phase 5 focuses on implementing the download decision making and import pipeline for books. This includes evaluating releases, downloading files, importing them into the library, and organizing them according to naming conventions.

## Tasks

### Task 5.1: Create Download & Import Documentation
- Document the download flow
- Define import pipeline stages
- Establish naming patterns

### Task 5.2: Implement Download Decision Maker
- Release evaluation logic
- Quality comparisons
- Upgrade decisions
- Custom format scoring

### Task 5.3: Create Book Import Service
- File detection and validation
- Book/Author matching
- Edition selection
- Import coordination

### Task 5.4: Implement File Name Builder
- Book file naming patterns
- Author folder structure
- Token replacement system
- Special character handling

### Task 5.5: Create Media File Service
- File management operations
- Move/copy operations
- Permission handling
- Cleanup of old files

### Task 5.6: Implement Import Decision Maker
- Import specifications
- Quality checks
- Existing file handling
- Upgrade logic

### Task 5.7: Create Post-Import Services
- Metadata embedding
- Cover extraction
- File tagging
- Library updates

### Task 5.8: Commit Phase 5 Changes
- Testing import pipeline
- Documentation updates
- Git commit and push

## Download Flow

```
Indexer Search Results
    ↓
Download Decision Maker
    ↓
Download Client
    ↓
Download Tracking
    ↓
Completed Download Handler
    ↓
Import Decision Maker
    ↓
Book Import Service
    ↓
Media File Service
    ↓
Post-Import Processing
```

## Import Pipeline Stages

### 1. Download Decision
- Parse release info
- Check if wanted
- Compare to existing
- Apply quality profile
- Check size limits
- Apply restrictions

### 2. Import Decision
- Validate file type
- Extract book info
- Match to library
- Check quality upgrade
- Apply import rules

### 3. File Import
- Move/copy file
- Apply naming format
- Set permissions
- Update database
- Trigger events

### 4. Post-Processing
- Extract metadata
- Embed tags
- Generate covers
- Update statistics
- Send notifications

## Naming Patterns

### Author Folder
```
{Author Name} ({Birth Year}-{Death Year})
{Author Name}
{Author SortName}
```

### Book File
```
{Author Name} - {Book Title} ({Release Year}) [{Quality}]
{Author Name} - {Series Title} #{Series Position} - {Book Title}
{Book Title} - {Author Name} [{Edition}]
```

### Token Examples
- `{Author Name}` → Stephen King
- `{Book Title}` → The Stand
- `{Release Year}` → 1978
- `{Quality}` → EPUB
- `{Edition}` → Unabridged
- `{Series Title}` → The Dark Tower
- `{Series Position}` → 01

## Quality Handling

### Book Formats (in order of preference)
1. EPUB (highest)
2. AZW3/MOBI
3. PDF
4. DJVU
5. CBR/CBZ (comics)
6. TXT (lowest)

### Audio Books
1. FLAC
2. MP3 320
3. MP3 256
4. MP3 192
5. MP3 128

## Import Specifications

### File Types
- eBooks: .epub, .mobi, .azw, .azw3, .pdf, .djvu, .fb2
- Audio: .mp3, .m4a, .m4b, .flac, .ogg
- Comics: .cbr, .cbz, .cb7
- Text: .txt, .rtf, .doc, .docx

### Metadata Sources
- Embedded metadata
- OPF files
- Calibre metadata.db
- File naming patterns
- NFO files

## Error Handling
- Invalid file formats
- Corrupted files
- Missing metadata
- Duplicate imports
- Permission errors
- Disk space issues