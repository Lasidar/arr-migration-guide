#!/bin/bash

echo "Fixing type references carefully..."

# Fix message types
find src -name "*.cs" -type f -exec sed -i 's/SeriesAddMessage/AuthorAddMessage/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SeriesDeleteMessage/AuthorDeleteMessage/g' {} \;

# Fix renamed file types
find src -name "*.cs" -type f -exec sed -i 's/RenamedEpisodeFile/RenamedBookFile/g' {} \;

# Fix namespace imports from Tv to Books for common types
find src -name "*.cs" -type f -exec sed -i 's/using Readarr\.Core\.Tv;/using Readarr.Core.Books;/g' {} \;

# Fix specific TV-related properties for ImportLists
find src -name "*.cs" -type f -exec sed -i 's/SearchForMissingEpisodes/SearchForMissingBooks/g' {} \;

echo "Type reference fixes completed."