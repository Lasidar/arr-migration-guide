using System;
using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore;
using Readarr.Core.MediaFiles;

namespace Readarr.Core.Books
{
    public class Edition : ModelBase, IComparable
    {
        public Edition()
        {
            Images = new List<MediaCover.MediaCover>();
        }

        // Foreign Keys
        public int BookId { get; set; }
        public int BookFileId { get; set; }
        
        // Edition Identifiers
        public string ForeignEditionId { get; set; } // Goodreads Edition ID
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Asin { get; set; }
        
        // Edition Info
        public string Title { get; set; }
        public string Language { get; set; }
        public string Overview { get; set; }
        public string Format { get; set; } // Hardcover, Paperback, eBook, Audiobook
        public bool IsEbook { get; set; }
        public string Publisher { get; set; }
        public int PageCount { get; set; }
        public DateTime? ReleaseDate { get; set; }
        
        // Monitoring
        public bool Monitored { get; set; }
        public bool ManualAdd { get; set; }
        
        // Metadata
        public Ratings Ratings { get; set; }
        public List<MediaCover.MediaCover> Images { get; set; }
        
        // System
        public DateTime? LastSearchTime { get; set; }
        
        // Relationships
        public LazyLoaded<BookFile> BookFile { get; set; }
        public Book Book { get; set; }

        public bool HasFile => BookFileId > 0;

        public override string ToString()
        {
            return string.Format("[{0}]{1}", Id, Title.NullSafe());
        }

        public int CompareTo(object obj)
        {
            var other = (Edition)obj;

            // Compare by release date
            if (ReleaseDate.HasValue && other.ReleaseDate.HasValue)
            {
                return ReleaseDate.Value.CompareTo(other.ReleaseDate.Value);
            }

            if (ReleaseDate.HasValue)
            {
                return -1;
            }

            if (other.ReleaseDate.HasValue)
            {
                return 1;
            }

            // If no release dates, compare by ID
            return Id.CompareTo(other.Id);
        }
    }
}