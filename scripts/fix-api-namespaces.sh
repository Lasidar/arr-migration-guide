#!/bin/bash

# Script to fix API namespaces from V3 to V1 in Readarr.Api.V1 project

echo "Fixing API namespaces in Readarr.Api.V1..."

cd /workspace/src/Readarr.Api.V1

# Fix namespace declarations
find . -name "*.cs" -type f | while read file; do
    sed -i 's/namespace Readarr\.Api\.V3/namespace Readarr.Api.V1/g' "$file"
done

# Fix V3ApiController attributes to V1ApiController
find . -name "*.cs" -type f | while read file; do
    sed -i 's/\[V3ApiController/[V1ApiController/g' "$file"
done

echo "API namespace fixes complete!"