#!/bin/bash

echo "Fixing all remaining ambiguous references..."

# Fix all Series references to Tv.Series
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/\bSeries series\b/Tv.Series series/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/(Series series/(Tv.Series series/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/, Series series/, Tv.Series series/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/ Series Series/ Tv.Series Series/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/<Series>/<Tv.Series>/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/List<Series>/List<Tv.Series>/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/IEnumerable<Series>/IEnumerable<Tv.Series>/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/readonly Series/readonly Tv.Series/g' {} \;

# Fix all ISeriesService references to Tv.ISeriesService
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/readonly ISeriesService/readonly Tv.ISeriesService/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/ISeriesService seriesService/Tv.ISeriesService seriesService/g' {} \;
find src/Readarr.Core -name "*.cs" -type f -exec sed -i 's/(ISeriesService/(Tv.ISeriesService/g' {} \;

# Fix LocalBook ambiguous references
sed -i 's/LocalBook localBook/Parser.Model.LocalBook localBook/g' src/Readarr.Core/MediaFiles/IUpgradeMediaFiles.cs

# Fix IMediaFileService ambiguous references
sed -i 's/readonly IMediaFileService/readonly MediaFiles.IMediaFileService/g' src/Readarr.Core/Extras/Metadata/Consumers/Plex/PlexMetadata.cs
sed -i 's/IMediaFileService mediaFileService/MediaFiles.IMediaFileService mediaFileService/g' src/Readarr.Core/Extras/Metadata/Consumers/Plex/PlexMetadata.cs

# Fix missing History.IHistoryService namespace
sed -i 's/Download.History.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/CompletedDownloadService.cs
sed -i 's/Download.History.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/TrackedDownloads/TrackedDownloadService.cs

# Fix missing BooksImportedEvent using
sed -i '/using Readarr.Core.Books.Events;/a using Readarr.Core.Books.Events;' src/Readarr.Core/HealthCheck/Checks/RecyclingBinCheck.cs
sed -i '/using Readarr.Core.Books.Events;/a using Readarr.Core.Books.Events;' src/Readarr.Core/Download/TrackedDownloads/DownloadMonitoringService.cs

# Fix missing Tv.Events using
sed -i '/using Readarr.Core.Tv;/a using Readarr.Core.Tv.Events;' src/Readarr.Core/HealthCheck/Checks/RemovedSeriesCheck.cs
sed -i '/using Readarr.Core.Tv;/a using Readarr.Core.Tv.Events;' src/Readarr.Core/HealthCheck/Checks/RootFolderCheck.cs
sed -i '/using Readarr.Core.Tv;/a using Readarr.Core.Tv.Events;' src/Readarr.Core/HealthCheck/Checks/ImportListRootFolderCheck.cs
sed -i '/using Readarr.Core.Tv;/a using Readarr.Core.Tv.Events;' src/Readarr.Core/DataAugmentation/Scene/SceneMappingService.cs

# Fix missing MediaFiles.Events using
sed -i '/using Readarr.Core.MediaFiles;/a using Readarr.Core.MediaFiles.Events;' src/Readarr.Core/Download/Pending/PendingReleaseService.cs

# Fix Goodreads BaseUrl
echo "public override string BaseUrl { get; set; } = \"https://www.goodreads.com\";" >> src/Readarr.Core/ImportLists/Goodreads/GoodreadsListSettings.cs

# Fix HttpBookImportListBase Enabled property
sed -i 's/public override bool Enabled/public bool Enabled/g' src/Readarr.Core/ImportLists/HttpBookImportListBase.cs

# Fix missing Author and Book in FileNameBuilder
sed -i '/using Readarr.Core.Books;/a using Readarr.Core.Books;' src/Readarr.Core/Organizer/FileNameBuilder.cs
sed -i '/using Readarr.Core.Books;/a using Readarr.Core.Books;' src/Readarr.Core/Organizer/IBuildFileNames.cs

# Remove duplicate using statements
find src/Readarr.Core -name "*.cs" -type f -exec awk '!seen[$0]++' {} > {}.tmp && mv {}.tmp {} \;

echo "All remaining ambiguous references fixed."