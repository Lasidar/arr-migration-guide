using System;
using System.Collections.Generic;
using System.Linq;
using Readarr.Api.V1.Author;
using Readarr.Api.V1.Book;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.Indexers;
using Readarr.Core.Languages;
using Readarr.Core.Qualities;
using Readarr.Core.Queue;
using Readarr.Api.V3.CustomFormats;
using Readarr.Http.REST;

namespace Readarr.Api.V1.Queue
{
    public class BookQueueResource : RestResource
    {
        public int? AuthorId { get; set; }
        public int? BookId { get; set; }
        public AuthorResource Author { get; set; }
        public BookResource Book { get; set; }
        public List<Language> Languages { get; set; }
        public QualityModel Quality { get; set; }
        public List<CustomFormatResource> CustomFormats { get; set; }
        public int CustomFormatScore { get; set; }
        public decimal Size { get; set; }
        public string Title { get; set; }

        public DateTime? EstimatedCompletionTime { get; set; }
        public DateTime? Added { get; set; }
        public QueueStatus Status { get; set; }
        public TrackedDownloadStatus? TrackedDownloadStatus { get; set; }
        public TrackedDownloadState? TrackedDownloadState { get; set; }
        public List<TrackedDownloadStatusMessage> StatusMessages { get; set; }
        public string ErrorMessage { get; set; }
        public string DownloadId { get; set; }
        public DownloadProtocol Protocol { get; set; }
        public string DownloadClient { get; set; }
        public bool DownloadClientHasPostImportCategory { get; set; }
        public string Indexer { get; set; }
        public string OutputPath { get; set; }
        public bool DownloadForced { get; set; }
    }

    public static class BookQueueResourceMapper
    {
        public static BookQueueResource ToResource(this Readarr.Core.Queue.Queue model, bool includeAuthor, bool includeBook)
        {
            if (model == null)
            {
                return null;
            }

            return new BookQueueResource
            {
                Id = model.Id,
                AuthorId = model.Author?.Id,
                BookId = model.Book?.Id,
                Author = includeAuthor && model.Author != null ? model.Author.ToResource() : null,
                Book = includeBook && model.Book != null ? model.Book.ToResource() : null,
                Languages = model.Languages,
                Quality = model.Quality,
                CustomFormats = model.RemoteBook?.CustomFormats?.ToResource(false),
                CustomFormatScore = model.Author?.QualityProfile?.Value?.CalculateCustomFormatScore(model.RemoteBook?.CustomFormats) ?? 0,
                Size = model.Size,
                Title = model.Title,
                EstimatedCompletionTime = model.EstimatedCompletionTime,
                Added = model.Added,
                Status = model.Status,
                TrackedDownloadStatus = model.TrackedDownloadStatus,
                TrackedDownloadState = model.TrackedDownloadState,
                StatusMessages = model.StatusMessages,
                ErrorMessage = model.ErrorMessage,
                DownloadId = model.DownloadId,
                Protocol = model.Protocol,
                DownloadClient = model.DownloadClient,
                DownloadClientHasPostImportCategory = model.DownloadClientHasPostImportCategory,
                Indexer = model.Indexer,
                OutputPath = model.OutputPath,
                DownloadForced = model.DownloadForced
            };
        }

        public static List<BookQueueResource> ToResource(this IEnumerable<Readarr.Core.Queue.Queue> models, bool includeAuthor, bool includeBook)
        {
            return models.Select(m => ToResource(m, includeAuthor, includeBook)).ToList();
        }
    }
}