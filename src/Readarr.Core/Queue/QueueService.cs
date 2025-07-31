using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Common.Crypto;
using Readarr.Core.Books;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.Languages;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Qualities;
using Readarr.Core.Books;

namespace Readarr.Core.Queue
{
    public interface IQueueService
    {
        List<Queue> GetQueue();
        Queue Find(int id);
        void Remove(int id);
    }

    public class QueueService : IQueueService, IHandle<TrackedDownloadRefreshedEvent>
    {
        private readonly IEventAggregator _eventAggregator;
        private static List<Queue> _queue = new();

        public QueueService(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public List<Queue> GetQueue()
        {
            return _queue;
        }

        public Queue Find(int id)
        {
            return _queue.SingleOrDefault(q => q.Id == id);
        }

        public void Remove(int id)
        {
            _queue.Remove(Find(id));
        }

        private IEnumerable<Queue> MapQueue(TrackedDownload trackedDownload)
        {
            // Handle books
            if (trackedDownload.RemoteBook?.Books != null && trackedDownload.RemoteBook.Books.Any())
            {
                foreach (var book in trackedDownload.RemoteBook.Books)
                {
                    yield return MapBookQueueItem(trackedDownload, book);
                }
            }
            // Handle TV episodes (to be removed)
            else if (trackedDownload.RemoteEpisode?.Episodes != null && trackedDownload.RemoteEpisode.Episodes.Any())
            {
                foreach (var episode in trackedDownload.RemoteEpisode.Episodes)
                {
                    yield return MapQueueItem(trackedDownload, episode);
                }
            }
            else
            {
                yield return MapQueueItem(trackedDownload, null);
            }
        }

        private Queue MapBookQueueItem(TrackedDownload trackedDownload, Book book)
        {
            var queue = new Queue
            {
                Author = trackedDownload.RemoteBook?.Author,
                Book = book,
                Languages = trackedDownload.RemoteBook?.Languages ?? new List<Language> { Language.Unknown },
                Quality = trackedDownload.RemoteBook?.Quality ?? new QualityModel(Quality.Unknown),
                Title = Parser.Parser.RemoveFileExtension(trackedDownload.DownloadItem.Title),
                Size = trackedDownload.DownloadItem.TotalSize,
                SizeLeft = trackedDownload.DownloadItem.RemainingSize,
                TimeLeft = trackedDownload.DownloadItem.RemainingTime,
                Status = Enum.TryParse(trackedDownload.DownloadItem.Status.ToString(), out QueueStatus outValue) ? outValue : QueueStatus.Unknown,
                TrackedDownloadStatus = trackedDownload.Status,
                TrackedDownloadState = trackedDownload.State,
                StatusMessages = trackedDownload.StatusMessages.ToList(),
                ErrorMessage = trackedDownload.DownloadItem.Message,
                RemoteBook = trackedDownload.RemoteBook,
                DownloadId = trackedDownload.DownloadItem.DownloadId,
                Protocol = trackedDownload.Protocol,
                DownloadClient = trackedDownload.DownloadItem.DownloadClientInfo.Name,
                DownloadClientHasPostImportCategory = trackedDownload.DownloadItem.DownloadClientInfo.HasPostImportCategory,
                Indexer = trackedDownload.Indexer,
                OutputPath = trackedDownload.DownloadItem.OutputPath.ToString()
            };

            if (trackedDownload.DownloadItem.RemainingTime != null)
            {
                queue.EstimatedCompletionTime = DateTime.UtcNow.Add(trackedDownload.DownloadItem.RemainingTime.Value);
            }

            return queue;
        }

        private Queue MapQueueItem(TrackedDownload trackedDownload, Episode episode)
        {
            var queue = new Queue
            {
                Series = trackedDownload.RemoteEpisode?.Series,
                Episode = episode,
                Languages = trackedDownload.RemoteEpisode?.Languages ?? new List<Language> { Language.Unknown },
                Quality = trackedDownload.RemoteEpisode?.ParsedEpisodeInfo.Quality ?? new QualityModel(Quality.Unknown),
                Title = Parser.Parser.RemoveFileExtension(trackedDownload.DownloadItem.Title),
                Size = trackedDownload.DownloadItem.TotalSize,
                SizeLeft = trackedDownload.DownloadItem.RemainingSize,
                TimeLeft = trackedDownload.DownloadItem.RemainingTime,
                Status = Enum.TryParse(trackedDownload.DownloadItem.Status.ToString(), out QueueStatus outValue) ? outValue : QueueStatus.Unknown,
                TrackedDownloadStatus = trackedDownload.Status,
                TrackedDownloadState = trackedDownload.State,
                StatusMessages = trackedDownload.StatusMessages.ToList(),
                ErrorMessage = trackedDownload.DownloadItem.Message,
                RemoteEpisode = trackedDownload.RemoteEpisode,
                DownloadId = trackedDownload.DownloadItem.DownloadId,
                Protocol = trackedDownload.Protocol,
                DownloadClient = trackedDownload.DownloadItem.DownloadClientInfo.Name,
                Indexer = trackedDownload.Indexer,
                OutputPath = trackedDownload.DownloadItem.OutputPath.ToString(),
                Added = trackedDownload.Added,
                DownloadClientHasPostImportCategory = trackedDownload.DownloadItem.DownloadClientInfo.HasPostImportCategory
            };

            queue.Id = HashConverter.GetHashInt31($"trackedDownload-{trackedDownload.DownloadClient}-{trackedDownload.DownloadItem.DownloadId}-ep{episode?.Id ?? 0}");

            if (queue.TimeLeft.HasValue)
            {
                queue.EstimatedCompletionTime = DateTime.UtcNow.Add(queue.TimeLeft.Value);
            }

            return queue;
        }

        public void Handle(TrackedDownloadRefreshedEvent message)
        {
            _queue = message.TrackedDownloads
                .Where(t => t.IsTrackable)
                .OrderBy(c => c.DownloadItem.RemainingTime)
                .SelectMany(MapQueue)
                .ToList();

            _eventAggregator.PublishEvent(new QueueUpdatedEvent());
        }
    }
}
