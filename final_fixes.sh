#!/bin/bash

echo "Applying final fixes..."

# Fix MediaInfoModel in EpisodeFile
sed -i '/using Readarr.Core.Qualities;/a using Readarr.Core.MediaFiles;' src/Readarr.Core/Tv/EpisodeFile.cs

# Fix duplicate BooksWithoutFiles method in BookService
sed -i '43d' src/Readarr.Core/Books/BookService.cs
sed -i '283d' src/Readarr.Core/Books/BookService.cs

# Fix ambiguous AuthorRenamedEvent in NotificationService
sed -i 's/IHandle<AuthorRenamedEvent>/IHandle<Books.Events.AuthorRenamedEvent>/g' src/Readarr.Core/Notifications/NotificationService.cs
sed -i 's/Handle(AuthorRenamedEvent/Handle(Books.Events.AuthorRenamedEvent/g' src/Readarr.Core/Notifications/NotificationService.cs

# Fix GoodreadsListSettings BaseUrl
sed -i '/public string BaseUrl/s/public string/public override string/' src/Readarr.Core/ImportLists/Goodreads/GoodreadsListSettings.cs

# Remove duplicate RenamedBookFiles in WebhookRenamePayload
sed -i '14d' src/Readarr.Core/Notifications/Webhook/WebhookRenamePayload.cs

# Fix ambiguous Series in FileNameSampleService
sed -i 's/private static readonly Series/private static readonly Tv.Series/g' src/Readarr.Core/Organizer/FileNameSampleService.cs
sed -i 's/GetSampleResult(Series series/GetSampleResult(Tv.Series series/g' src/Readarr.Core/Organizer/FileNameSampleService.cs

# Fix ambiguous ISeriesService in BookSeriesValidator
sed -i 's/private readonly ISeriesService/private readonly Books.ISeriesService/g' src/Readarr.Core/Validation/BookSeriesValidator.cs
sed -i 's/public BookSeriesCollectionValidator(ISeriesService/public BookSeriesCollectionValidator(Books.ISeriesService/g' src/Readarr.Core/Validation/BookSeriesValidator.cs

# Fix DownloadMessage Tv.Series
sed -i 's/public Tv.Tv.Series/public Tv.Series/g' src/Readarr.Core/Notifications/DownloadMessage.cs

# Fix ambiguous Series in CustomFormats
sed -i 's/ParseCustomFormat(Series series/ParseCustomFormat(Tv.Series series/g' src/Readarr.Core/CustomFormats/CustomFormatCalculationService.cs
sed -i 's/ParseCustomFormat(LocalEpisode localEpisode, Series series/ParseCustomFormat(LocalEpisode localEpisode, Tv.Series series/g' src/Readarr.Core/CustomFormats/CustomFormatCalculationService.cs
sed -i 's/ParseCustomFormat(RemoteEpisode remoteEpisode, Series series/ParseCustomFormat(RemoteEpisode remoteEpisode, Tv.Series series/g' src/Readarr.Core/CustomFormats/CustomFormatCalculationService.cs
sed -i 's/ParseCustomFormat(EpisodeFile episodeFile, Series series/ParseCustomFormat(EpisodeFile episodeFile, Tv.Series series/g' src/Readarr.Core/CustomFormats/CustomFormatCalculationService.cs
sed -i 's/ParseCustomFormat(EpisodeFile episodeFile, List<Episode> episodes, Series series/ParseCustomFormat(EpisodeFile episodeFile, List<Episode> episodes, Tv.Series series/g' src/Readarr.Core/CustomFormats/CustomFormatCalculationService.cs
sed -i 's/ParseCustomFormat(Blocklist blocklist, Series series/ParseCustomFormat(Blocklist blocklist, Tv.Series series/g' src/Readarr.Core/CustomFormats/CustomFormatCalculationService.cs
sed -i 's/ParseCustomFormat(EpisodeHistory history, Series series/ParseCustomFormat(EpisodeHistory history, Tv.Series series/g' src/Readarr.Core/CustomFormats/CustomFormatCalculationService.cs
sed -i 's/public Series Series/public Tv.Series Series/g' src/Readarr.Core/CustomFormats/CustomFormatInput.cs

# Fix ambiguous ISeriesService in DiskSpaceService
sed -i 's/private readonly ISeriesService/private readonly Tv.ISeriesService/g' src/Readarr.Core/DiskSpace/DiskSpaceService.cs
sed -i 's/ISeriesService seriesService/Tv.ISeriesService seriesService/g' src/Readarr.Core/DiskSpace/DiskSpaceService.cs

# Fix ImportDecision in BooksImportedEvent
sed -i 's/List<ImportDecision<LocalBook>>/List<MediaFiles.BookImport.ImportDecision<Parser.Model.LocalBook>>/g' src/Readarr.Core/Books/Events/BooksImportedEvent.cs

# Fix ambiguous Series in ExtraService and others
sed -i 's/ImportExtraFiles(Series series/ImportExtraFiles(Tv.Series series/g' src/Readarr.Core/Extras/ExtraService.cs
sed -i 's/ImportExtraFiles(Series series/ImportExtraFiles(Tv.Series series/g' src/Readarr.Core/Extras/ExistingExtraFileService.cs
sed -i 's/public Series Series/public Tv.Series Series/g' src/Readarr.Core/IndexerSearch/Definitions/SearchCriteriaBase.cs

# Remove duplicate using directives
sed -i '/^using Readarr.Core.Books;$/d; /^using Readarr.Core.Books;$/i using Readarr.Core.Books;' src/Readarr.Core/Organizer/FileNameBuilder.cs
sed -i '/^using Readarr.Core.Books;$/d; /^using Readarr.Core.Books;$/i using Readarr.Core.Books;' src/Readarr.Core/Organizer/IBuildFileNames.cs

echo "Final fixes applied."