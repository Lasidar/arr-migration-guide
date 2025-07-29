#!/bin/bash

# Create a copy of the original patch
cp readarrv2-complete-transformation.patch readarrv2-complete-transformation-fixed.patch

# Fix the API directory renames that were already done in Phase 1
sed -i 's|src/Sonarr\.Api\.V3/|src/Readarr.Api.V1/|g' readarrv2-complete-transformation-fixed.patch
sed -i 's|src/Sonarr\.Api\.V5/|src/Readarr.Api.V3/|g' readarrv2-complete-transformation-fixed.patch
sed -i 's|src/Sonarr\.Http/|src/Readarr.Http/|g' readarrv2-complete-transformation-fixed.patch
sed -i 's|src/Sonarr\.RuntimePatches/|src/Readarr.RuntimePatches/|g' readarrv2-complete-transformation-fixed.patch

# Fix the solution file rename
sed -i 's|src/Sonarr\.sln|src/Readarr.sln|g' readarrv2-complete-transformation-fixed.patch

# Fix project file renames
sed -i 's|/Sonarr\.Api\.V3\.csproj|/Readarr.Api.V1.csproj|g' readarrv2-complete-transformation-fixed.patch
sed -i 's|/Sonarr\.Api\.V5\.csproj|/Readarr.Api.V3.csproj|g' readarrv2-complete-transformation-fixed.patch
sed -i 's|/Sonarr\.Http\.csproj|/Readarr.Http.csproj|g' readarrv2-complete-transformation-fixed.patch
sed -i 's|/Sonarr\.RuntimePatches\.csproj|/Readarr.RuntimePatches.csproj|g' readarrv2-complete-transformation-fixed.patch

# Fix the Tv to Books directory rename
sed -i 's|src/NzbDrone\.Core/Tv/|src/NzbDrone.Core/Books/|g' readarrv2-complete-transformation-fixed.patch

echo "Patch file has been fixed and saved as readarrv2-complete-transformation-fixed.patch"