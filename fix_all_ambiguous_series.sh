#!/bin/bash

echo "Fixing all ambiguous Series references..."

# Fix in notification files
find src/Readarr.Core/Notifications -name "*.cs" -type f -exec sed -i 's/\bSeries Series\b/Tv.Series Series/g' {} \;
find src/Readarr.Core/Notifications -name "*.cs" -type f -exec sed -i 's/(Series series/(Tv.Series series/g' {} \;
find src/Readarr.Core/Notifications -name "*.cs" -type f -exec sed -i 's/, Series series/, Tv.Series series/g' {} \;
find src/Readarr.Core/Notifications -name "*.cs" -type f -exec sed -i 's/List<Series>/List<Tv.Series>/g' {} \;
find src/Readarr.Core/Notifications -name "*.cs" -type f -exec sed -i 's/IEnumerable<Series>/IEnumerable<Tv.Series>/g' {} \;

# Fix in AutoTagging files
find src/Readarr.Core/AutoTagging -name "*.cs" -type f -exec sed -i 's/\bSeries series\b/Tv.Series series/g' {} \;
find src/Readarr.Core/AutoTagging -name "*.cs" -type f -exec sed -i 's/List<Series>/List<Tv.Series>/g' {} \;
find src/Readarr.Core/AutoTagging -name "*.cs" -type f -exec sed -i 's/IAutoTaggingTest(Series/IAutoTaggingTest(Tv.Series/g' {} \;

# Fix in Blocklisting
sed -i 's/public Series Series/public Tv.Series Series/g' src/Readarr.Core/Blocklisting/Blocklist.cs

# Fix duplicate ReleaseType
rm -f src/Readarr.Core/Parser/Model/ReleaseType.cs

# Fix duplicate PathValidator
rm -f src/Readarr.Core/Validation/Paths/PathValidator.cs

# Fix MediaInfoModel in EpisodeFile
sed -i '1i using Readarr.Core.MediaFiles;' src/Readarr.Core/Tv/EpisodeFile.cs

echo "Ambiguous references fixed."