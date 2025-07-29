#!/bin/bash

# Script to update project references in .csproj files

echo "Starting project reference updates..."

cd /workspace/src

# Find all .csproj files and update references
find . -name "*.csproj" -type f | while read file; do
    echo "Updating references in: $file"
    
    # Update folder paths from NzbDrone to Readarr
    sed -i 's|/NzbDrone\.|/Readarr.|g' "$file"
    sed -i 's|\\NzbDrone\.|\\Readarr.|g' "$file"
    
    # Update project file names from Sonarr to Readarr
    sed -i 's|Sonarr\.Common\.csproj|Readarr.Common.csproj|g' "$file"
    sed -i 's|Sonarr\.Core\.csproj|Readarr.Core.csproj|g' "$file"
    sed -i 's|Sonarr\.Api\.csproj|Readarr.Api.csproj|g' "$file"
    sed -i 's|Sonarr\.Host\.csproj|Readarr.Host.csproj|g' "$file"
    sed -i 's|Sonarr\.SignalR\.csproj|Readarr.SignalR.csproj|g' "$file"
    sed -i 's|Sonarr\.Test\.Common\.csproj|Readarr.Test.Common.csproj|g' "$file"
    sed -i 's|Sonarr\.Test\.Dummy\.csproj|Readarr.Test.Dummy.csproj|g' "$file"
    sed -i 's|Sonarr\.Windows\.csproj|Readarr.Windows.csproj|g' "$file"
    sed -i 's|Sonarr\.Mono\.csproj|Readarr.Mono.csproj|g' "$file"
    sed -i 's|Sonarr\.Update\.csproj|Readarr.Update.csproj|g' "$file"
    sed -i 's|Sonarr\.Console\.csproj|Readarr.Console.csproj|g' "$file"
    sed -i 's|Sonarr\.Common\.Test\.csproj|Readarr.Common.Test.csproj|g' "$file"
    sed -i 's|Sonarr\.Windows\.Test\.csproj|Readarr.Windows.Test.csproj|g' "$file"
    sed -i 's|Sonarr\.Mono\.Test\.csproj|Readarr.Mono.Test.csproj|g' "$file"
    
    # Update any remaining Sonarr references in project files
    sed -i 's|Sonarr\.|Readarr.|g' "$file"
done

echo "Project reference updates complete!"