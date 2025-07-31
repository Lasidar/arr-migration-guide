#!/bin/bash

echo "Fixing ambiguous references..."

# Fix CompletedDownloadService ambiguous ISeriesService
sed -i 's/private readonly ISeriesService _seriesService;/private readonly Tv.ISeriesService _seriesService;/g' src/Readarr.Core/Download/CompletedDownloadService.cs
sed -i 's/ISeriesService seriesService,/Tv.ISeriesService seriesService,/g' src/Readarr.Core/Download/CompletedDownloadService.cs

# Fix ImportListSyncService ambiguous ISeriesService
sed -i 's/private readonly ISeriesService _seriesService;/private readonly Tv.ISeriesService _seriesService;/g' src/Readarr.Core/ImportLists/ImportListSyncService.cs
sed -i 's/ISeriesService seriesService,/Tv.ISeriesService seriesService,/g' src/Readarr.Core/ImportLists/ImportListSyncService.cs

# Fix ambiguous Series in UntrackedDownloadCompletedEvent
sed -i 's/public Series Series/public Tv.Series Series/g' src/Readarr.Core/Download/UntrackedDownloadCompletedEvent.cs
sed -i 's/public UntrackedDownloadCompletedEvent(Series series/public UntrackedDownloadCompletedEvent(Tv.Series series/g' src/Readarr.Core/Download/UntrackedDownloadCompletedEvent.cs

# Fix ambiguous Series in DownloadMessage
sed -i 's/public Series Series/public Tv.Series Series/g' src/Readarr.Core/Notifications/DownloadMessage.cs

# Fix ambiguous LocalBook in UpgradeMediaFileService
sed -i 's/public UpgradeResult UpgradeEpisodeFile(LocalBook localBook/public UpgradeResult UpgradeEpisodeFile(Parser.Model.LocalBook localBook/g' src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs
sed -i 's/List<LocalBook> localBooks/List<Parser.Model.LocalBook> localBooks/g' src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs

# Fix IMediaFileService ambiguous references in ExtraService
sed -i 's/private readonly IMediaFileService _mediaFileService;/private readonly MediaFiles.IMediaFileService _mediaFileService;/g' src/Readarr.Core/Extras/ExtraService.cs
sed -i 's/IMediaFileService mediaFileService,/MediaFiles.IMediaFileService mediaFileService,/g' src/Readarr.Core/Extras/ExtraService.cs

# Fix IMediaFileService ambiguous references in PlexMetadata
sed -i 's/private readonly IMediaFileService _mediaFileService;/private readonly MediaFiles.IMediaFileService _mediaFileService;/g' src/Readarr.Core/Extras/Metadata/Consumers/Plex/PlexMetadata.cs
sed -i 's/IMediaFileService mediaFileService,/MediaFiles.IMediaFileService mediaFileService,/g' src/Readarr.Core/Extras/Metadata/Consumers/Plex/PlexMetadata.cs

# Fix ambiguous Series in MediaFileService
sed -i 's/GetFilesBySeries(int seriesId, Series series)/GetFilesBySeries(int seriesId, Tv.Series series)/g' src/Readarr.Core/MediaFiles/MediaFileService.cs

echo "Ambiguous references fixed."