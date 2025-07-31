#!/bin/bash

echo "Fixing type references..."

# Fix message types
find src -name "*.cs" -type f -exec sed -i 's/SeriesAddMessage/AuthorAddMessage/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SeriesDeleteMessage/AuthorDeleteMessage/g' {} \;

# Fix renamed file types
find src -name "*.cs" -type f -exec sed -i 's/RenamedEpisodeFile/RenamedBookFile/g' {} \;

# Fix Series references to Author
find src -name "*.cs" -type f -exec sed -i 's/\bSeries\b/Author/g' {} \;

# Fix Season references to Book
find src -name "*.cs" -type f -exec sed -i 's/\bSeason\b/Book/g' {} \;

# Fix Episode references to Edition
find src -name "*.cs" -type f -exec sed -i 's/\bEpisode\b/Edition/g' {} \;

# Fix SeriesTypes to AuthorTypes (if exists)
find src -name "*.cs" -type f -exec sed -i 's/SeriesTypes/AuthorTypes/g' {} \;

# Fix namespace imports from Tv to Books for common types
find src -name "*.cs" -type f -exec sed -i 's/using Readarr\.Core\.Tv;/using Readarr.Core.Books;/g' {} \;

# Fix specific TV-related properties
find src -name "*.cs" -type f -exec sed -i 's/SeasonFolder/BookFolder/g' {} \;
find src -name "*.cs" -type f -exec sed -i 's/SearchForMissingEpisodes/SearchForMissingBooks/g' {} \;

echo "Type reference fixes completed."