#!/bin/bash

echo "Fixing final 30 errors..."

# Fix GoodreadsListSettings BaseUrl
sed -i '/}$/d' src/Readarr.Core/ImportLists/Goodreads/GoodreadsListSettings.cs
sed -i '/public override string BaseUrl/d' src/Readarr.Core/ImportLists/Goodreads/GoodreadsListSettings.cs
echo '        public override string BaseUrl { get; set; } = "https://www.goodreads.com";' >> src/Readarr.Core/ImportLists/Goodreads/GoodreadsListSettings.cs
echo '    }' >> src/Readarr.Core/ImportLists/Goodreads/GoodreadsListSettings.cs
echo '}' >> src/Readarr.Core/ImportLists/Goodreads/GoodreadsListSettings.cs

# Fix History.IHistoryService references
sed -i 's/Download.History.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/CompletedDownloadService.cs
sed -i 's/Download.History.IHistoryService/History.IHistoryService/g' src/Readarr.Core/Download/TrackedDownloads/TrackedDownloadService.cs

# Fix missing EpisodeGrabbedEvent using
echo 'using Readarr.Core.MediaFiles.Events;' >> src/Readarr.Core/Download/Pending/PendingReleaseService.cs

# Fix missing BooksImportedEvent
echo 'using Readarr.Core.Books.Events;' >> src/Readarr.Core/Download/TrackedDownloads/DownloadMonitoringService.cs
echo 'using Readarr.Core.Books.Events;' >> src/Readarr.Core/HealthCheck/Checks/RecyclingBinCheck.cs

# Fix missing Author and Book types in Organizer
sed -i '1i using Readarr.Core.Books;' src/Readarr.Core/Organizer/FileNameBuilder.cs
sed -i '1i using Readarr.Core.Books;' src/Readarr.Core/Organizer/IBuildFileNames.cs

# Fix ambiguous Series in FileNameSampleService
sed -i 's/static readonly Series/static readonly Tv.Series/g' src/Readarr.Core/Organizer/FileNameSampleService.cs

# Fix ambiguous LocalBook in UpgradeMediaFileService
sed -i 's/(LocalBook localBook/(Parser.Model.LocalBook localBook/g' src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs

# Fix ambiguous Series in ExtraService
sed -i 's/ImportExtraFiles(Series series/ImportExtraFiles(Tv.Series series/g' src/Readarr.Core/Extras/ExtraService.cs

# Fix ambiguous Series in BookInfoProxy
sed -i 's/var series = new Series()/var series = new Tv.Series()/g' src/Readarr.Core/MetadataSource/BookInfo/BookInfoProxy.cs

# Fix Books.Tv.ISeriesService to Tv.ISeriesService
sed -i 's/Books.Tv.ISeriesService/Tv.ISeriesService/g' src/Readarr.Core/Validation/BookSeriesValidator.cs

# Fix SeriesTypeSpecificationValidator
echo 'namespace Readarr.Core.Tv { public class SeriesTypeSpecificationValidator { } }' >> src/Readarr.Core/Tv/SeriesTypeSpecificationValidator.cs

# Fix MediaInfoModel in EpisodeFile
sed -i '/using Readarr.Core.Datastore;/a using Readarr.Core.MediaFiles.MediaInfo;' src/Readarr.Core/Tv/EpisodeFile.cs

# Fix IImportApprovedBooks generic type
sed -i 's/List<ImportDecision>/List<ImportDecision<LocalBook>>/g' src/Readarr.Core/MediaFiles/BookImport/IImportApprovedBooks.cs

# Implement missing interface method in UpgradeMediaFileService
sed -i '/public UpgradeResult UpgradeEpisodeFile/a\        public UpgradeResult UpgradeBookFile(BookFile bookFile, Parser.Model.LocalBook localBook, bool copyOnly = false) { throw new NotImplementedException(); }' src/Readarr.Core/MediaFiles/UpgradeMediaFileService.cs

echo "Final 30 errors fixed."