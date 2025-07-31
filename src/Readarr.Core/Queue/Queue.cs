using System;
using System.Collections.Generic;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.Download.TrackedDownloads;
using Readarr.Core.Indexers;
using Readarr.Core.Languages;
using Readarr.Core.Parser.Model;
using Readarr.Core.Qualities;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.Queue
{
    public class Queue : ModelBase
    {
        // Book properties
        public Author Author { get; set; }
        public Book Book { get; set; }
        public RemoteBook RemoteBook { get; set; }
        
        // TV properties (to be removed)
        public Tv.Series Series { get; set; }
        public Episode Episode { get; set; }
        public RemoteEpisode RemoteEpisode { get; set; }
        
        // Common properties
        public List<Language> Languages { get; set; }
        public QualityModel Quality { get; set; }
        public decimal Size { get; set; }
        public string Title { get; set; }
        public decimal SizeLeft { get; set; }
        public TimeSpan? TimeLeft { get; set; }
        public DateTime? EstimatedCompletionTime { get; set; }
        public DateTime? Added { get; set; }
        public QueueStatus Status { get; set; }
        public TrackedDownloadStatus? TrackedDownloadStatus { get; set; }
        public TrackedDownloadState? TrackedDownloadState { get; set; }
        public List<TrackedDownloadStatusMessage> StatusMessages { get; set; }
        public string DownloadId { get; set; }
        public DownloadProtocol Protocol { get; set; }
        public string DownloadClient { get; set; }
        public bool DownloadClientHasPostImportCategory { get; set; }
        public string Indexer { get; set; }
        public string OutputPath { get; set; }
        public string ErrorMessage { get; set; }
    }
}
