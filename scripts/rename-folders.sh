#!/bin/bash

# Script to rename NzbDrone folders to Readarr

echo "Starting folder renaming..."

cd /workspace/src

# Rename all NzbDrone.* directories to Readarr.*
for dir in NzbDrone.*; do
    if [ -d "$dir" ]; then
        newdir=$(echo "$dir" | sed 's/NzbDrone/Readarr/')
        if [ ! -e "$newdir" ]; then
            echo "Renaming $dir to $newdir"
            mv "$dir" "$newdir"
        else
            echo "Skipping $dir - $newdir already exists"
        fi
    fi
done

# Special case for NzbDrone directory (without dot)
if [ -d "NzbDrone" ]; then
    if [ ! -e "Readarr" ]; then
        echo "Renaming NzbDrone to Readarr"
        mv "NzbDrone" "Readarr"
    else
        echo "Skipping NzbDrone - Readarr already exists"
    fi
fi

echo "Folder renaming complete!"