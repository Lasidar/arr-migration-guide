using System;
using System.Collections.Generic;
using Readarr.Core.Datastore;

namespace Readarr.Core.AuthorStats
{
    public class AuthorStatistics : ResultSet
    {
        public int AuthorId { get; set; }
        public int BookCount { get; set; }
        public int BookFileCount { get; set; }
        public int BookCountWithFiles { get; set; }
        public int TotalBookCount { get; set; }
        public int AvailableBookCount { get; set; }
        public int MonitoredBookCount { get; set; }
        public int UnmonitoredBookCount { get; set; }
        public long SizeOnDisk { get; set; }
        
        public List<BookStatistics> BookStatistics { get; set; }
    }

    public class BookStatistics : ResultSet
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public int BookFileCount { get; set; }
        public int EditionCount { get; set; }
        public int EditionFileCount { get; set; }
        public int TotalEditionCount { get; set; }
        public long SizeOnDisk { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? PreviousAiring { get; set; }
        public bool Monitored { get; set; }
    }
}