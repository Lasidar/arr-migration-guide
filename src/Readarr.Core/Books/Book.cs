using System;
using System.Collections.Generic;
using Readarr.Core.Datastore;

namespace Readarr.Core.Books
{
    public class Book : ModelBase
    {
        public Book()
        {
            Editions = new List<Edition>();
        }

        // Foreign Keys
        public int AuthorId { get; set; }
        
        // Metadata
        public int BookMetadataId { get; set; }
        public LazyLoaded<BookMetadata> Metadata { get; set; }
        
        // Book Info (local overrides)
        public string CleanTitle { get; set; }
        public string SortTitle { get; set; }
        public bool Monitored { get; set; }
        public bool AnyEditionOk { get; set; }
        
        // Series Info (if part of a series)
        public int? SeriesId { get; set; }
        public string SeriesPosition { get; set; }
        
        // Relationships
        public LazyLoaded<Author> Author { get; set; }
        public List<Edition> Editions { get; set; }
        
        // System
        public DateTime Added { get; set; }
        public DateTime? LastInfoSync { get; set; }
        public AddBookOptions AddOptions { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]: {1}", Metadata.Value?.ForeignBookId, Metadata.Value?.Title ?? "Unknown");
        }

        public void ApplyChanges(Book otherBook)
        {
            BookMetadataId = otherBook.BookMetadataId;
            
            Editions = otherBook.Editions;
            Monitored = otherBook.Monitored;
            AnyEditionOk = otherBook.AnyEditionOk;
            
            SeriesId = otherBook.SeriesId;
            SeriesPosition = otherBook.SeriesPosition;
            
            AddOptions = otherBook.AddOptions;
        }
    }

    public class Links : IEmbeddedDocument
    {
        public string Url { get; set; }
        public string Name { get; set; }
    }

    public class AddBookOptions : MonitoringOptions
    {
        public bool SearchForNewBook { get; set; }
    }
}