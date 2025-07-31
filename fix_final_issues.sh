#!/bin/bash

echo "Fixing final issues..."

# Fix History.IHistoryService references
sed -i 's/Download\.History\.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/CompletedDownloadService.cs
sed -i 's/Download\.History\.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/TrackedDownloads/TrackedDownloadService.cs

# Fix ambiguous Series in FileNameSampleService  
sed -i 's/private static readonly Tv\.Series/private static readonly Tv.Series/g' src/Readarr.Core/Organizer/FileNameSampleService.cs

# Fix ambiguous Series in ExtraService
sed -i 's/ImportExtraFiles(Tv\.Series series/ImportExtraFiles(Tv.Series series/g' src/Readarr.Core/Extras/ExtraService.cs

# Fix ambiguous Series in BookInfoProxy
sed -i 's/var series = new Tv\.Series()/var series = new Tv.Series()/g' src/Readarr.Core/MetadataSource/BookInfo/BookInfoProxy.cs

# Fix ambiguous LocalBook in UpgradeMediaFileService
sed -i 's/UpgradeEpisodeFile(Parser\.Model\.LocalBook/UpgradeEpisodeFile(Parser.Model.LocalBook/g' src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs

# Fix UpgradeBookFile return type
sed -i 's/public UpgradeResult UpgradeBookFile/public BookFileUpgradeResult UpgradeBookFile/g' src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs

echo "Final issues fixed."