# Readarr v2 Test Migration Status

## Current State

The main Readarr codebase has been successfully migrated and builds without compilation errors. However, the test projects have many compilation errors due to the fundamental changes in the domain model from TV-centric (Series/Episodes) to book-centric (Authors/Books).

## Test Compilation Status

- **Main Code**: ✅ Builds successfully (0 CS errors)
- **Test Code**: ❌ 338+ CS errors

## Common Test Issues

### 1. Map Method Signature Changes
The `IParsingService.Map` method has been changed from accepting `ParsedEpisodeInfo` to `BookInfo`. Many tests are still trying to use the old signature with 5 arguments.

**Example Error:**
```
No overload for method 'Map' takes 5 arguments
Argument 1: cannot convert from 'ParsedEpisodeInfo' to 'BookInfo'
```

### 2. DownloadDecision Constructor Changes
`DownloadDecision` now supports both `RemoteBook` and `RemoteEpisode`, but many tests need updating to use the correct constructor.

### 3. Rejection/ImportRejection Constructor Changes
The constructor signature changed from `(Reason, Message)` to `(Message, RejectionType)`.

### 4. Missing or Renamed Interfaces
- `IMakeDownloadDecision` → `IDownloadDecisionMaker`
- `DownloadRejection` → `Rejection`

### 5. TV-Specific Test Logic
Many tests are inherently TV-specific and need complete rewriting for the book domain:
- Episode-based tests need to be converted to Edition-based
- Series-based tests need to be converted to Author-based
- Season logic needs to be replaced with Book logic

## Fixes Applied

1. ✅ Fixed ImportRejection constructor calls
2. ✅ Replaced DownloadRejection with Rejection
3. ✅ Fixed ambiguous DownloadDecision constructors
4. ✅ Renamed IMakeDownloadDecision to IDownloadDecisionMaker
5. ✅ Fixed Rejection.Message to Rejection.Reason
6. ✅ Added missing using directives
7. ✅ Fixed GetSeriesFolder method signatures
8. ✅ Added GetBackupFolder to IBackupService

## Next Steps

1. **Prioritize Core Functionality**: Focus on getting the main application running rather than fixing all tests immediately.

2. **Create Book-Specific Tests**: Rather than trying to adapt TV tests, create new tests specifically for the book domain model.

3. **Temporarily Disable TV Tests**: Consider excluding TV-specific test projects from the build until they can be properly migrated.

4. **Incremental Test Migration**: Migrate tests module by module, starting with the most critical functionality.

## Recommendation

Given that the main codebase builds successfully, it's recommended to:

1. Run the application and fix any runtime issues
2. Create minimal book-specific tests for critical paths
3. Gradually migrate or replace TV tests over time

The extensive test failures are expected given the fundamental domain model changes. The priority should be on validating that the core application works correctly with the new book-centric model.