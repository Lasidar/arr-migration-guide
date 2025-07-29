#!/bin/bash

# Script to transform Sonarr references to Readarr
# This script performs global search and replace operations

echo "Starting Sonarr to Readarr transformation..."

# Create backup directory
mkdir -p /workspace/backup
echo "Created backup directory"

# Function to perform replacements in a file
replace_in_file() {
    local file=$1
    
    # Skip binary files and certain directories
    if [[ -f "$file" ]] && file --mime "$file" | grep -q "text/"; then
        # Create backup
        cp "$file" "/workspace/backup/$(basename $file).bak" 2>/dev/null || true
        
        # Perform replacements
        sed -i 's/Sonarr/Readarr/g' "$file"
        sed -i 's/sonarr/readarr/g' "$file"
        sed -i 's/SONARR/READARR/g' "$file"
        sed -i 's/NzbDrone/Readarr/g' "$file"
        sed -i 's/nzbdrone/readarr/g' "$file"
        sed -i 's/NZBDRONE/READARR/g' "$file"
        
        # Specific replacements for series/episode to book/author
        sed -i 's/Series/Book/g' "$file"
        sed -i 's/series/book/g' "$file"
        sed -i 's/SERIES/BOOK/g' "$file"
        sed -i 's/Episode/Chapter/g' "$file"
        sed -i 's/episode/chapter/g' "$file"
        sed -i 's/EPISODE/CHAPTER/g' "$file"
        sed -i 's/Season/Volume/g' "$file"
        sed -i 's/season/volume/g' "$file"
        sed -i 's/SEASON/VOLUME/g' "$file"
        sed -i 's/TV Show/Book/g' "$file"
        sed -i 's/tv show/book/g' "$file"
        sed -i 's/TV SHOW/BOOK/g' "$file"
        sed -i 's/TvShow/Book/g' "$file"
        sed -i 's/tvshow/book/g' "$file"
        sed -i 's/TVSHOW/BOOK/g' "$file"
        
        echo "Processed: $file"
    fi
}

# Find all text files and process them
export -f replace_in_file
find /workspace/src -type f \( -name "*.cs" -o -name "*.csproj" -o -name "*.json" -o -name "*.config" -o -name "*.xml" -o -name "*.md" -o -name "*.txt" -o -name "*.yml" -o -name "*.yaml" \) -exec bash -c 'replace_in_file "$0"' {} \;

echo "Transformation complete!"
echo "Backup files created in /workspace/backup"