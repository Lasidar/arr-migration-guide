#!/bin/bash

# Script to transform namespaces from NzbDrone to Readarr
# This is more targeted than the general transformation

echo "Starting namespace transformation..."

# Function to transform namespaces in C# files
transform_namespaces() {
    local file=$1
    
    if [[ -f "$file" ]] && [[ "$file" == *.cs ]]; then
        # Transform namespace declarations
        sed -i 's/namespace NzbDrone/namespace Readarr/g' "$file"
        sed -i 's/using NzbDrone/using Readarr/g' "$file"
        
        # Transform specific namespace references
        sed -i 's/NzbDrone\.Common/Readarr.Common/g' "$file"
        sed -i 's/NzbDrone\.Core/Readarr.Core/g' "$file"
        sed -i 's/NzbDrone\.Api/Readarr.Api/g' "$file"
        sed -i 's/NzbDrone\.Host/Readarr.Host/g' "$file"
        sed -i 's/NzbDrone\.SignalR/Readarr.SignalR/g' "$file"
        sed -i 's/NzbDrone\.Test/Readarr.Test/g' "$file"
        sed -i 's/NzbDrone\.Windows/Readarr.Windows/g' "$file"
        sed -i 's/NzbDrone\.Mono/Readarr.Mono/g' "$file"
        sed -i 's/NzbDrone\.Update/Readarr.Update/g' "$file"
        sed -i 's/NzbDrone\.Console/Readarr.Console/g' "$file"
        sed -i 's/NzbDrone\.Integration/Readarr.Integration/g' "$file"
        sed -i 's/NzbDrone\.Libraries/Readarr.Libraries/g' "$file"
        sed -i 's/NzbDrone\.Automation/Readarr.Automation/g' "$file"
        
        # Transform class names that include NzbDrone
        sed -i 's/NzbDroneException/ReadarrException/g' "$file"
        sed -i 's/NzbDroneLogger/ReadarrLogger/g' "$file"
        sed -i 's/NzbDroneProcessService/ReadarrProcessService/g' "$file"
        
        # Transform ISonarrCloudRequestBuilder to IReadarrCloudRequestBuilder
        sed -i 's/ISonarrCloudRequestBuilder/IReadarrCloudRequestBuilder/g' "$file"
        sed -i 's/SonarrCloudRequestBuilder/ReadarrCloudRequestBuilder/g' "$file"
        
        echo "Transformed namespaces in: $file"
    fi
}

# Export the function so it can be used with find -exec
export -f transform_namespaces

# Find all C# files and transform them
find /workspace/src -name "*.cs" -exec bash -c 'transform_namespaces "$0"' {} \;

echo "Namespace transformation complete!"