using System;
using System.Collections.Generic;
using NzbDrone.Core.Datastore;

namespace NzbDrone.Core.Books
{
    public class Book : ModelBase
    {
        public Book()
        {
            Images = new List<MediaCover.MediaCover>();
            Editions = new List<Edition>();
        }

        public int AuthorId { get; set; }
        public int BookNumber { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public int PageCount { get; set; }
        public DateTime? PublishDate { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string Overview { get; set; }
        public bool Monitored { get; set; }
        public List<MediaCover.MediaCover> Images { get; set; }
        public BookStatistics Statistics { get; set; }
        public List<Edition> Editions { get; set; }
    }

    public class BookStatistics : ResultSet
    {
        public int EditionFileCount { get; set; }
        public int EditionCount { get; set; }
        public int TotalEditionCount { get; set; }
        public long SizeOnDisk { get; set; }
        public decimal PercentOfEditions
        {
            get
            {
                if (EditionCount == 0)
                {
                    return 0;
                }

                return EditionCount / (decimal)TotalEditionCount * 100;
            }
        }
    }
}
