using System;
using System.Collections.Generic;
using NzbDrone.Common.Extensions;
using NzbDrone.Core.Datastore;
using NzbDrone.Core.MediaFiles;

namespace NzbDrone.Core.Books
{
    public class Edition : ModelBase, IComparable
    {
        public Edition()
        {
            Images = new List<MediaCover.MediaCover>();
        }

        public const string PUBLISH_DATE_FORMAT = "yyyy-MM-dd";

        public int AuthorId { get; set; }
        public int BookId { get; set; }
        public int GoodreadsEditionId { get; set; }
        public int EditionFileId { get; set; }
        public int BookNumber { get; set; }
        public int EditionNumber { get; set; }
        public string Title { get; set; }
        public string Isbn { get; set; }
        public string Isbn13 { get; set; }
        public string Format { get; set; }
        public int PageCount { get; set; }
        public string Publisher { get; set; }
        public string Language { get; set; }
        public string PublishDate { get; set; }
        public DateTime? PublishDateUtc { get; set; }
        public string Overview { get; set; }
        public bool Monitored { get; set; }
        public int? AbsoluteEditionNumber { get; set; }
        public int? SceneAbsoluteEditionNumber { get; set; }
        public int? SceneBookNumber { get; set; }
        public int? SceneEditionNumber { get; set; }
        public bool UnverifiedSceneNumbering { get; set; }
        public Ratings Ratings { get; set; }
        public List<MediaCover.MediaCover> Images { get; set; }
        public DateTime? LastSearchTime { get; set; }

        public string AuthorTitle { get; private set; }

        public LazyLoaded<EditionFile> EditionFile { get; set; }

        public Author Author { get; set; }

        public bool HasFile => EditionFileId > 0;

        public override string ToString()
        {
            return string.Format("[{0}] {1}", GoodreadsEditionId, Title.NullSafe());
        }

        public int CompareTo(object obj)
        {
            var other = obj as Edition;

            if (other == null)
            {
                return 1;
            }

            if (BookNumber != other.BookNumber)
            {
                return BookNumber.CompareTo(other.BookNumber);
            }

            return EditionNumber.CompareTo(other.EditionNumber);
        }
    }
}
