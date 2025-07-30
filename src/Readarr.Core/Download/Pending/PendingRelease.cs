using System;
using Readarr.Core.Books;
using Readarr.Core.Datastore;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.Download.Pending
{
    public class PendingRelease : ModelBase
    {
        public int SeriesId { get; set; }
        public string Title { get; set; }
        public DateTime Added { get; set; }
        public ParsedEpisodeInfo ParsedEpisodeInfo { get; set; }
        public ReleaseInfo Release { get; set; }
        public PendingReleaseReason Reason { get; set; }
        public PendingReleaseAdditionalInfo AdditionalInfo { get; set; }

        // Not persisted
        public RemoteEpisode RemoteEpisode { get; set; }
        
        // Book-related properties (not persisted)
        public RemoteBook RemoteBook { get; set; }
        public ParsedBookInfo ParsedBookInfo { get; set; }
        public Author Author { get; set; }
    }

    public class PendingReleaseAdditionalInfo
    {
        public SeriesMatchType SeriesMatchType { get; set; }
        public ReleaseSourceType ReleaseSource { get; set; }
    }
}
