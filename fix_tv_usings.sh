#!/bin/bash

echo "Fixing missing Tv using statements..."

# Add using Readarr.Core.Tv; to files that need it
find src -name "ImportCompleteMessage.cs" -exec sed -i '/using Readarr.Core.Books;/a using Readarr.Core.Tv;' {} \;
find src -name "DownloadMessage.cs" -exec sed -i '/using Readarr.Core.Books;/a using Readarr.Core.Tv;' {} \;
find src -name "Discord.cs" -exec sed -i '/using Readarr.Core.Books.Events;/a using Readarr.Core.Tv;' {} \;
find src -name "ImportListItemInfo.cs" -exec sed -i '/using Readarr.Core.Books;/a using Readarr.Core.Tv;' {} \;
find src -name "SeriesTypeSpecification.cs" -exec sed -i '/namespace Readarr.Core.AutoTagging.Specifications/i using Readarr.Core.Tv;' {} \;
find src -name "StatusSpecification.cs" -exec sed -i '/namespace Readarr.Core.AutoTagging.Specifications/i using Readarr.Core.Tv;' {} \;

echo "Tv using statements fixed."
