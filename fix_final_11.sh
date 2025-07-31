#!/bin/bash

echo "Fixing final 11 errors..."

# Fix History namespace issue
sed -i 's/Download.History.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/CompletedDownloadService.cs
sed -i 's/Download.History.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/TrackedDownloads/TrackedDownloadService.cs

# Fix ambiguous Series in FileNameSampleService (missed some)
sed -i 's/private static readonly Series/private static readonly Tv.Series/g' src/Readarr.Core/Organizer/FileNameSampleService.cs

# Fix ambiguous Series in ExtraService  
sed -i 's/(Series series/(Tv.Series series/g' src/Readarr.Core/Extras/ExtraService.cs

# Fix ambiguous Series in BookInfoProxy
sed -i 's/= new Series()/= new Tv.Series()/g' src/Readarr.Core/MetadataSource/BookInfo/BookInfoProxy.cs

# Fix ambiguous LocalBook in UpgradeMediaFileService
sed -i 's/UpgradeEpisodeFile(LocalBook/UpgradeEpisodeFile(Parser.Model.LocalBook/g' src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs

# Add missing UpgradeBookFile implementation
cat << 'EOF' >> src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs

        public UpgradeResult UpgradeBookFile(BookFile bookFile, Parser.Model.LocalBook localBook, bool copyOnly = false)
        {
            // Stub implementation for TV compatibility
            throw new NotImplementedException();
        }
EOF

echo "Final 11 errors fixed."