using System;
using System.Collections.Generic;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.Languages;
using NzbDrone.Core.Profiles.Qualities;

namespace NzbDrone.Core.Books
{
    public class Author : ModelBase
    {
        public Author()
        {
            Images = new List<MediaCover.MediaCover>();
            Genres = new List<string>();
            Books = new List<Book>();
            Tags = new HashSet<int>();
            OriginalLanguage = Language.English;
        }

        public int GoodreadsId { get; set; }
        public string AmazonId { get; set; }
        public string WikipediaId { get; set; }
        public string IsbnId { get; set; }
        public string Name { get; set; }
        public string CleanName { get; set; }
        public string SortName { get; set; }
        public AuthorStatusType Status { get; set; }
        public string Biography { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }
        public bool Monitored { get; set; }
        public NewItemMonitorTypes MonitorNewItems { get; set; }
        public int QualityProfileId { get; set; }
        public bool BookFolder { get; set; }
        public DateTime? LastInfoSync { get; set; }
        public List<MediaCover.MediaCover> Images { get; set; }
        public AuthorTypes AuthorType { get; set; }
        public string Publisher { get; set; }
        public string NameSlug { get; set; }
        public string Path { get; set; }
        public int Year { get; set; }
        public Ratings Ratings { get; set; }
        public List<string> Genres { get; set; }
        public List<Book> Books { get; set; }
        public DateTime Added { get; set; }
        public AddAuthorOptions AddOptions { get; set; }
        public HashSet<int> Tags { get; set; }
        public Language OriginalLanguage { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", GoodreadsId, Name.NullSafe());
        }

        public void ApplyChanges(Author otherAuthor)
        {
            GoodreadsId = otherAuthor.GoodreadsId;
            Books = otherAuthor.Books;
            Path = otherAuthor.Path;
            QualityProfileId = otherAuthor.QualityProfileId;

            BookFolder = otherAuthor.BookFolder;
            Monitored = otherAuthor.Monitored;
            MonitorNewItems = otherAuthor.MonitorNewItems;

            NameSlug = otherAuthor.NameSlug;
            Name = otherAuthor.Name;
            CleanName = otherAuthor.CleanName;
            SortName = otherAuthor.SortName;
            Status = otherAuthor.Status;
            Biography = otherAuthor.Biography;
            BirthDate = otherAuthor.BirthDate;
            DeathDate = otherAuthor.DeathDate;
            Images = otherAuthor.Images;
            Genres = otherAuthor.Genres;
            Ratings = otherAuthor.Ratings;
            AuthorType = otherAuthor.AuthorType;
            Publisher = otherAuthor.Publisher;
            Year = otherAuthor.Year;
            Added = otherAuthor.Added;
            AddOptions = otherAuthor.AddOptions;
            Tags = otherAuthor.Tags;
            OriginalLanguage = otherAuthor.OriginalLanguage;
        }
    }

    public enum AuthorTypes
    {
        Standard = 0,
        Daily = 1,
        Anthology = 2
    }

    public enum AuthorStatusType
    {
        Continuing = 0,
        Ended = 1,
        Upcoming = 2,
        Deleted = 3
    }
}
