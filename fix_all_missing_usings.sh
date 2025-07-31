#!/bin/bash

echo "Adding missing Tv using statements to all files..."

# Find all .cs files that reference Episode, EpisodeFile, Series, Season but don't have using Readarr.Core.Tv
files_needing_tv=$(find src -name "*.cs" -type f | while read file; do
    if grep -qE "(Episode|EpisodeFile|Series|Season|SeriesTypes)" "$file" && ! grep -q "using Readarr.Core.Tv;" "$file"; then
        echo "$file"
    fi
done)

for file in $files_needing_tv; do
    # Add using Readarr.Core.Tv; after the last using statement
    if grep -q "^using " "$file"; then
        # Get the line number of the last using statement
        last_using=$(grep -n "^using " "$file" | tail -1 | cut -d: -f1)
        if [ -n "$last_using" ]; then
            sed -i "${last_using}a\\using Readarr.Core.Tv;" "$file"
            echo "Added Tv using to $file"
        fi
    fi
done

echo "Tv using statements added."