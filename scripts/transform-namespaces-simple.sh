#!/bin/bash

# Simple script to transform namespaces from NzbDrone to Readarr

echo "Starting namespace transformation..."

# Transform all C# files
find /workspace/src -name "*.cs" -type f | while read file; do
    # Transform namespace declarations and using statements
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
done

echo "Namespace transformation complete!"