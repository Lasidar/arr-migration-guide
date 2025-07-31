#!/bin/bash

# Fix namespace imports
echo "Fixing namespace imports..."

# Replace Tv.Events with Books.Events
find src -name "*.cs" -type f -exec sed -i 's/using Readarr\.Core\.Tv\.Events;/using Readarr.Core.Books.Events;/g' {} \;

# Replace EpisodeImport with BookImport
find src -name "*.cs" -type f -exec sed -i 's/using Readarr\.Core\.MediaFiles\.EpisodeImport;/using Readarr.Core.MediaFiles.BookImport;/g' {} \;

# Replace common TV event types with Book event types
find src -name "*.cs" -type f -exec sed -i 's/SeriesAddedEvent/AuthorAddedEvent/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SeriesDeletedEvent/AuthorDeletedEvent/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SeriesEditedEvent/AuthorEditedEvent/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SeriesUpdatedEvent/AuthorUpdatedEvent/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SeriesRenamedEvent/AuthorRenamedEvent/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SeriesImportedEvent/AuthorsImportedEvent/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/EpisodeImportedEvent/BooksImportedEvent/g' {} \;

echo "Namespace fixes completed."