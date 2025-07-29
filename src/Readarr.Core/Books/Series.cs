using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore;

namespace Readarr.Core.Books
{
    public class Series : ModelBase
    {
        public Series()
        {
        }

        // Series Identifiers
        public string ForeignSeriesId { get; set; } // Goodreads Series ID
        
        // Series Info
        public string Title { get; set; }
        public string CleanTitle { get; set; }
        public string SortTitle { get; set; }
        public string Description { get; set; }
        public bool Numbered { get; set; } // Whether books in series are numbered
        
        // Foreign Keys
        public int AuthorId { get; set; }
        
        // Relationships
        public LazyLoaded<Author> Author { get; set; }
        public LazyLoaded<List<SeriesBookLink>> Books { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]: {1}", ForeignSeriesId, Title.NullSafe());
        }
    }

    public class SeriesBookLink : ModelBase
    {
        public int SeriesId { get; set; }
        public int BookId { get; set; }
        public string Position { get; set; } // Can be "1", "1.5", "A", etc.
        public int? PositionOrder { get; set; } // For sorting
        
        public LazyLoaded<Series> Series { get; set; }
        public LazyLoaded<Book> Book { get; set; }
    }
}