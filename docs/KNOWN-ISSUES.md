# Known Issues & Limitations

## Critical Issues

### 1. Backup Restore Not Implemented
**Severity**: High  
**Impact**: Cannot restore from backups  
**Location**: `src/Readarr.Core/Backup/BackupService.cs:140`  
**Details**: The restore functionality throws `NotImplementedException`. Backups can be created but not restored.  
**Workaround**: Manual database file restoration  
**Fix Required**: Implement unzip and file restoration logic

### 2. Quality Profile Validation
**Severity**: Medium  
**Impact**: Quality profile checks may not work correctly  
**Location**: `src/Readarr.Core/Books/AddAuthorService.cs:166`  
**Details**: Quality profile validation is commented with TODO  
**Workaround**: Manual validation before adding authors  
**Fix Required**: Implement proper quality profile service

## Functional Limitations

### 1. Simplified Metadata Profiles
**Impact**: Limited metadata selection options  
**Details**: Basic implementation compared to full Sonarr functionality  
**Enhancement**: Add granular metadata selection

### 2. Custom Format Support
**Impact**: Limited custom format functionality for books  
**Details**: Book formats differ significantly from video formats  
**Enhancement**: Design book-specific custom format system

### 3. Manual Import
**Impact**: Simplified compared to Sonarr's implementation  
**Details**: Basic file matching without advanced options  
**Enhancement**: Add interactive import mode

### 4. Series Management
**Impact**: Basic series linking functionality  
**Details**: No automatic series detection or numbering validation  
**Enhancement**: Add series metadata providers

## Technical Debt

### 1. Incomplete Interfaces
Several interfaces have basic implementations:
- `IMediaFileService` - Simplified version
- `IHistoryService` - Stub implementation  
- `IRootFolderWatchingService` - Empty interface

### 2. Missing Validations
- ISBN validation is basic (no checksum validation)
- ASIN format validation missing
- Author name duplicate checking is case-sensitive

### 3. Database Migrations
- No migration from Readarr v1 to v2
- Manual database setup required
- Missing indexes for performance

### 4. Error Handling
Some areas need better error handling:
- Metadata provider failures
- File system permission errors
- Network timeout handling

## Performance Considerations

### 1. Large Library Scanning
**Issue**: Full library scans can be slow  
**Impact**: Libraries with 10,000+ books  
**Mitigation**: Implement incremental scanning

### 2. Metadata Refresh
**Issue**: No batching for metadata requests  
**Impact**: API rate limiting possible  
**Mitigation**: Add request queuing and batching

### 3. Database Queries
**Issue**: Some N+1 query patterns exist  
**Impact**: Slow API responses with large datasets  
**Mitigation**: Add eager loading where needed

## API Limitations

### 1. Pagination
**Issue**: Not all endpoints support pagination  
**Impact**: Large result sets can timeout  
**Affected Endpoints**:
- `/api/v1/book` (when no authorId specified)
- `/api/v1/edition`

### 2. Filtering
**Issue**: Limited filter options compared to Sonarr  
**Impact**: Cannot do complex queries  
**Enhancement**: Add OData or GraphQL support

### 3. Bulk Operations
**Issue**: Limited bulk operation support  
**Impact**: Slow for mass updates  
**Enhancement**: Add bulk endpoints

## Import/Export Issues

### 1. Calibre Integration
**Status**: Not implemented  
**Impact**: No Calibre library import  
**Enhancement**: Add Calibre metadata.db reader

### 2. Reading List Import
**Status**: Not implemented  
**Impact**: Cannot import from Goodreads shelves  
**Enhancement**: Add import list providers

### 3. OPDS Support
**Status**: Not implemented  
**Impact**: No OPDS catalog generation  
**Enhancement**: Add OPDS feed generator

## Platform-Specific Issues

### 1. Linux Permissions
**Issue**: Permission setting is basic  
**Impact**: May not handle complex permission scenarios  
**Enhancement**: Add ACL support

### 2. Windows Path Handling
**Issue**: Long path support not tested  
**Impact**: Paths > 260 chars may fail  
**Enhancement**: Enable long path support

### 3. Docker Volumes
**Issue**: No special Docker handling  
**Impact**: Permission issues in containers  
**Enhancement**: Add Docker detection and handling

## Security Considerations

### 1. API Authentication
**Status**: Using existing Sonarr auth  
**Consideration**: May need book-specific permissions

### 2. File Access
**Status**: No sandboxing  
**Consideration**: Can access any readable path

### 3. External API Keys
**Status**: Stored in plain text  
**Consideration**: Should encrypt sensitive data

## Workarounds & Temporary Solutions

### 1. Manual Metadata Refresh
```bash
# Force refresh all authors
curl -X POST http://localhost:7878/api/v1/command \
  -H "X-Api-Key: YOUR_API_KEY" \
  -d '{"name": "RefreshAuthor"}'
```

### 2. Database Cleanup
```sql
-- Remove orphaned book files
DELETE FROM BookFiles 
WHERE BookId NOT IN (SELECT Id FROM Books);

-- Remove orphaned metadata
DELETE FROM BookMetadata 
WHERE Id NOT IN (SELECT BookMetadataId FROM Books);
```

### 3. Permission Fix
```bash
# Fix permissions on Linux
find /path/to/library -type d -exec chmod 755 {} \;
find /path/to/library -type f -exec chmod 644 {} \;
```

## Monitoring & Debugging

### 1. Enable Debug Logging
Add to config.xml:
```xml
<LogLevel>Debug</LogLevel>
```

### 2. Check Database Integrity
```sql
PRAGMA integrity_check;
PRAGMA foreign_key_check;
```

### 3. API Response Times
Monitor slow endpoints:
- `/api/v1/author` with large libraries
- `/api/v1/book/search` with complex queries

## Future Improvements

### High Priority
1. Implement backup restore
2. Add Calibre integration
3. Improve series management
4. Add bulk operations

### Medium Priority
1. OPDS support
2. Reading progress tracking
3. Enhanced duplicate detection
4. Better ISBN validation

### Low Priority
1. Social features
2. Recommendation engine
3. E-reader sync
4. Reading statistics

## Getting Help

### Log Locations
- Windows: `%APPDATA%\Readarr\logs`
- Linux: `/home/user/.config/Readarr/logs`
- Docker: `/config/logs`

### Debug Information
When reporting issues, include:
1. Readarr version
2. OS and version
3. Database type (SQLite/PostgreSQL)
4. Library size (authors/books)
5. Relevant log entries

### Community Support
- GitHub Issues: Report bugs and feature requests
- Discord: Real-time help
- Forums: Detailed discussions
- Wiki: Documentation and guides