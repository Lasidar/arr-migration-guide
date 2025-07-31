using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Readarr.Common.Cache;
using Readarr.Common.Extensions;
using Readarr.Core.CustomFormats;
using Readarr.Core.Download.Aggregation;
using Readarr.Core.Download.History;
using Readarr.Core.History;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser;
using Readarr.Core.Parser.Model;
using Readarr.Core.Books;
using Readarr.Core.Books.Events;
using Readarr.Core.Tv;
using Readarr.Core.Tv.Events;

namespace Readarr.Core.Download.TrackedDownloads
{
    public interface ITrackedDownloadService
    {
        TrackedDownload Find(string downloadId);
        void StopTracking(string downloadId);
        void StopTracking(List<string> downloadIds);
        TrackedDownload TrackDownload(DownloadClientDefinition downloadClient, DownloadClientItem downloadItem);
        List<TrackedDownload> GetTrackedDownloads();
        void UpdateTrackable(List<TrackedDownload> trackedDownloads);
    }

    public class TrackedDownloadService : ITrackedDownloadService,
                                          IHandle<EpisodeInfoRefreshedEvent>,
                                          IHandle<AuthorAddedEvent>,
                                          IHandle<AuthorEditedEvent>,
                                          IHandle<SeriesBulkEditedEvent>,
                                          IHandle<AuthorDeletedEvent>
    {
        private readonly IParsingService _parsingService;
        private readonly Readarr.Core.History.IHistoryService _historyService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IDownloadHistoryService _downloadHistoryService;
        private readonly IRemoteEpisodeAggregationService _aggregationService;
        private readonly ICustomFormatCalculationService _formatCalculator;
        private readonly Logger _logger;
        private readonly ICached<TrackedDownload> _cache;

        public TrackedDownloadService(IParsingService parsingService,
                                      ICacheManager cacheManager,
                                      Readarr.Core.History.IHistoryService historyService,
                                      ICustomFormatCalculationService formatCalculator,
                                      IEventAggregator eventAggregator,
                                      IDownloadHistoryService downloadHistoryService,
                                      IRemoteEpisodeAggregationService aggregationService,
                                      Logger logger)
        {
            _parsingService = parsingService;
            _historyService = historyService;
            _formatCalculator = formatCalculator;
            _eventAggregator = eventAggregator;
            _downloadHistoryService = downloadHistoryService;
            _aggregationService = aggregationService;
            _cache = cacheManager.GetCache<TrackedDownload>(GetType());
            _logger = logger;
        }

        public TrackedDownload Find(string downloadId)
        {
            return _cache.Find(downloadId);
        }

        public void StopTracking(string downloadId)
        {
            var trackedDownload = _cache.Find(downloadId);

            _cache.Remove(downloadId);
            _eventAggregator.PublishEvent(new TrackedDownloadsRemovedEvent(new List<TrackedDownload> { trackedDownload }));
        }

        public void StopTracking(List<string> downloadIds)
        {
            var trackedDownloads = new List<TrackedDownload>();

            foreach (var downloadId in downloadIds)
            {
                var trackedDownload = _cache.Find(downloadId);
                _cache.Remove(downloadId);
                trackedDownloads.Add(trackedDownload);
            }

            _eventAggregator.PublishEvent(new TrackedDownloadsRemovedEvent(trackedDownloads));
        }

        public TrackedDownload TrackDownload(DownloadClientDefinition downloadClient, DownloadClientItem downloadItem)
        {
            var existingItem = Find(downloadItem.DownloadId);

            if (existingItem != null && existingItem.State != TrackedDownloadState.Downloading)
            {
                LogItemChange(existingItem, existingItem.DownloadItem, downloadItem);

                existingItem.DownloadItem = downloadItem;
                existingItem.IsTrackable = true;

                return existingItem;
            }

            var trackedDownload = new TrackedDownload
            {
                DownloadClient = downloadClient.Id,
                DownloadItem = downloadItem,
                Protocol = downloadClient.Protocol,
                IsTrackable = true,
                HasNotifiedManualInteractionRequired = existingItem?.HasNotifiedManualInteractionRequired ?? false
            };

            try
            {
                var historyItems = _historyService.FindByDownloadId(downloadItem.DownloadId)
                    .OrderByDescending(h => h.Date)
                    .ToList();

                var bookInfo = Parser.Parser.ParseBookTitle(trackedDownload.DownloadItem.Title);

                if (bookInfo != null)
                {
                    trackedDownload.RemoteBook = _parsingService.Map(bookInfo, null);
                }

                var downloadHistory = _downloadHistoryService.GetLatestDownloadHistoryItem(downloadItem.DownloadId);

                if (downloadHistory != null)
                {
                    var state = GetStateFromHistory(downloadHistory.EventType);
                    trackedDownload.State = state;
                }

                if (historyItems.Any())
                {
                    var firstHistoryItem = historyItems.First();
                    var grabbedEvent = historyItems.FirstOrDefault(v => v.EventType == EpisodeHistoryEventType.Grabbed);

                    trackedDownload.Indexer = grabbedEvent?.Data?.GetValueOrDefault("indexer");
                    trackedDownload.Added = grabbedEvent?.Date;

                    if (bookInfo == null ||
                        trackedDownload.RemoteBook?.Author == null ||
                        !trackedDownload.RemoteBook.Books.Any())
                    {
                        // Try parsing the original source title and if that fails, try parsing it as a special
                        // TODO: Implement special book title parsing if needed
                        bookInfo = Parser.Parser.ParseBookTitle(firstHistoryItem.SourceTitle);

                        if (bookInfo != null)
                        {
                            trackedDownload.RemoteBook = _parsingService.Map(bookInfo,
                                firstHistoryItem.SeriesId,
                                historyItems.Where(v => v.EventType == EpisodeHistoryEventType.Grabbed)
                                    .Select(h => h.EpisodeId).Distinct());
                        }
                    }

                    if (trackedDownload.RemoteBook != null)
                    {
                        trackedDownload.RemoteBook.Release ??= new ReleaseInfo();
                        trackedDownload.RemoteBook.Release.Indexer = trackedDownload.Indexer;
                        trackedDownload.RemoteBook.Release.Title = trackedDownload.RemoteBook.ParsedBookInfo?.ReleaseTitle;

                        if (Enum.TryParse(grabbedEvent?.Data?.GetValueOrDefault("indexerFlags"), true, out IndexerFlags flags))
                        {
                            trackedDownload.RemoteBook.Release.IndexerFlags = flags;
                        }

                        if (downloadHistory != null)
                        {
                            trackedDownload.RemoteBook.Release.IndexerId = downloadHistory.IndexerId;
                        }
                    }
                }

                if (trackedDownload.RemoteBook != null)
                {
                    // TODO: Implement RemoteBookAggregationService
                    // _aggregationService.Augment(trackedDownload.RemoteBook);

                    // Calculate custom formats
                    // TODO: Update format calculator for books
                    // trackedDownload.RemoteBook.CustomFormats = _formatCalculator.ParseCustomFormat(trackedDownload.RemoteBook, downloadItem.TotalSize);
                }

                // Track it so it can be displayed in the queue even though we can't determine which series it is for
                if (trackedDownload.RemoteBook == null)
                {
                    _logger.Trace("No Episode found for download '{0}'", trackedDownload.DownloadItem.Title);
                }
            }
            catch (MultipleSeriesFoundException e)
            {
                _logger.Debug(e, "Found multiple series for " + downloadItem.Title);

                trackedDownload.Warn("Unable to import automatically, found multiple series: {0}", string.Join(", ", e.Series));
            }
            catch (Exception e)
            {
                _logger.Debug(e, "Failed to find episode for " + downloadItem.Title);

                trackedDownload.Warn("Unable to parse episodes from title");
            }

            LogItemChange(trackedDownload, existingItem?.DownloadItem, trackedDownload.DownloadItem);

            _cache.Set(trackedDownload.DownloadItem.DownloadId, trackedDownload);
            return trackedDownload;
        }

        public List<TrackedDownload> GetTrackedDownloads()
        {
            return _cache.Values.ToList();
        }

        public void UpdateTrackable(List<TrackedDownload> trackedDownloads)
        {
            var untrackable = GetTrackedDownloads().ExceptBy(t => t.DownloadItem.DownloadId, trackedDownloads, t => t.DownloadItem.DownloadId, StringComparer.CurrentCulture).ToList();

            foreach (var trackedDownload in untrackable)
            {
                trackedDownload.IsTrackable = false;
            }
        }

        private void LogItemChange(TrackedDownload trackedDownload, DownloadClientItem existingItem, DownloadClientItem downloadItem)
        {
            if (existingItem == null ||
                existingItem.Status != downloadItem.Status ||
                existingItem.CanBeRemoved != downloadItem.CanBeRemoved ||
                existingItem.CanMoveFiles != downloadItem.CanMoveFiles)
            {
                _logger.Debug("Tracking '{0}:{1}': ClientState={2}{3} SonarrStage={4} Episode='{5}' OutputPath={6}.",
                    downloadItem.DownloadClientInfo.Name,
                    downloadItem.Title,
                    downloadItem.Status,
                    downloadItem.CanBeRemoved ? "" : downloadItem.CanMoveFiles ? " (busy)" : " (readonly)",
                    trackedDownload.State,
                    trackedDownload.RemoteEpisode?.ParsedEpisodeInfo,
                    downloadItem.OutputPath);
            }
        }

        private void UpdateCachedItem(TrackedDownload trackedDownload)
        {
            var bookInfo = Parser.Parser.ParseBookTitle(trackedDownload.DownloadItem.Title);

            trackedDownload.RemoteBook = bookInfo == null ? null : _parsingService.Map(bookInfo, null);

            if (trackedDownload.RemoteBook != null)
            {
                // TODO: Implement RemoteBookAggregationService
                // _aggregationService.Augment(trackedDownload.RemoteBook);
            }
        }

        private static TrackedDownloadState GetStateFromHistory(DownloadHistoryEventType eventType)
        {
            switch (eventType)
            {
                case DownloadHistoryEventType.DownloadImported:
                    return TrackedDownloadState.Imported;
                case DownloadHistoryEventType.DownloadFailed:
                    return TrackedDownloadState.Failed;
                case DownloadHistoryEventType.DownloadIgnored:
                    return TrackedDownloadState.Ignored;
                default:
                    return TrackedDownloadState.Downloading;
            }
        }

        public void Handle(EpisodeInfoRefreshedEvent message)
        {
            var needsToUpdate = false;

            foreach (var episode in message.Removed)
            {
                var cachedItems = _cache.Values.Where(t =>
                                            t.RemoteEpisode?.Episodes != null &&
                                            t.RemoteEpisode.Episodes.Any(e => e.Id == episode.Id))
                                        .ToList();

                if (cachedItems.Any())
                {
                    needsToUpdate = true;
                }

                cachedItems.ForEach(UpdateCachedItem);
            }

            if (needsToUpdate)
            {
                _eventAggregator.PublishEvent(new TrackedDownloadRefreshedEvent(GetTrackedDownloads()));
            }
        }

        public void Handle(AuthorAddedEvent message)
        {
            var cachedItems = _cache.Values
                .Where(t =>
                    t.RemoteEpisode?.Series == null ||
                    message.Series?.TvdbId == t.RemoteEpisode.Series.TvdbId)
                .ToList();

            if (cachedItems.Any())
            {
                cachedItems.ForEach(UpdateCachedItem);

                _eventAggregator.PublishEvent(new TrackedDownloadRefreshedEvent(GetTrackedDownloads()));
            }
        }

        public void Handle(AuthorEditedEvent message)
        {
            var cachedItems = _cache.Values
                .Where(t =>
                    t.RemoteEpisode?.Series != null &&
                    (t.RemoteEpisode.Series.Id == message.Series?.Id || t.RemoteEpisode.Series.TvdbId == message.Series?.TvdbId))
                .ToList();

            if (cachedItems.Any())
            {
                cachedItems.ForEach(UpdateCachedItem);

                _eventAggregator.PublishEvent(new TrackedDownloadRefreshedEvent(GetTrackedDownloads()));
            }
        }

        public void Handle(SeriesBulkEditedEvent message)
        {
            var cachedItems = _cache.Values
                .Where(t =>
                    t.RemoteEpisode?.Series != null &&
                    message.Series.Any(s => s.Id == t.RemoteEpisode.Series.Id || s.TvdbId == t.RemoteEpisode.Series.TvdbId))
                .ToList();

            if (cachedItems.Any())
            {
                cachedItems.ForEach(UpdateCachedItem);

                _eventAggregator.PublishEvent(new TrackedDownloadRefreshedEvent(GetTrackedDownloads()));
            }
        }

        public void Handle(AuthorDeletedEvent message)
        {
            var cachedItems = _cache.Values
                .Where(t =>
                    t.RemoteEpisode?.Series != null &&
                    message.Series.Any(s => s.Id == t.RemoteEpisode.Series.Id || s.TvdbId == t.RemoteEpisode.Series.TvdbId))
                .ToList();

            if (cachedItems.Any())
            {
                cachedItems.ForEach(UpdateCachedItem);

                _eventAggregator.PublishEvent(new TrackedDownloadRefreshedEvent(GetTrackedDownloads()));
            }
        }
    }
}
