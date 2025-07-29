using System;
using System.Collections.Generic;
using Readarr.Common.Extensions;
using Readarr.Core.Datastore;
using Readarr.Core.Profiles.Qualities;

namespace Readarr.Core.Books
{
    public class Author : ModelBase
    {
        public Author()
        {
            Tags = new HashSet<int>();
        }

        // Metadata
        public int AuthorMetadataId { get; set; }
        public LazyLoaded<AuthorMetadata> Metadata { get; set; }
        
        // Basic Info (local overrides)
        public string CleanName { get; set; }
        public string SortName { get; set; }
        
        // Monitoring
        public bool Monitored { get; set; }
        public NewItemMonitorTypes MonitorNewItems { get; set; }
        public int QualityProfileId { get; set; }
        public int MetadataProfileId { get; set; }
        
        // File Management
        public DateTime? LastInfoSync { get; set; }
        public string Path { get; set; }
        
        // System
        public string RootFolderPath { get; set; }
        public DateTime Added { get; set; }
        public LazyLoaded<QualityProfile> QualityProfile { get; set; }
        
        // Relationships
        public LazyLoaded<List<Book>> Books { get; set; }
        public LazyLoaded<List<Series>> Series { get; set; }
        public HashSet<int> Tags { get; set; }
        public AddAuthorOptions AddOptions { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Metadata.Value?.ForeignAuthorId, Metadata.Value?.Name ?? "Unknown");
        }

        public void ApplyChanges(Author otherAuthor)
        {
            AuthorMetadataId = otherAuthor.AuthorMetadataId;
            
            Path = otherAuthor.Path;
            QualityProfileId = otherAuthor.QualityProfileId;
            MetadataProfileId = otherAuthor.MetadataProfileId;
            
            Monitored = otherAuthor.Monitored;
            MonitorNewItems = otherAuthor.MonitorNewItems;
            
            RootFolderPath = otherAuthor.RootFolderPath;
            Tags = otherAuthor.Tags;
            AddOptions = otherAuthor.AddOptions;
        }
    }

    public enum AuthorStatusType
    {
        Continuing,
        Deceased,
        Paused
    }
}