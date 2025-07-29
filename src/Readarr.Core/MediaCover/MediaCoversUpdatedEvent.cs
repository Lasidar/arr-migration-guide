using Readarr.Common.Messaging;
using Readarr.Core.Books;
using Readarr.Core.Tv;

namespace Readarr.Core.MediaCover
{
    public class MediaCoversUpdatedEvent : IEvent
    {
        public Series Series { get; set; }
        public Author Author { get; set; }
        public Book Book { get; set; }
        public bool Updated { get; set; }

        public MediaCoversUpdatedEvent(Series series, bool updated)
        {
            Series = series;
            Updated = updated;
        }

        public MediaCoversUpdatedEvent(Author author, bool updated)
        {
            Author = author;
            Updated = updated;
        }

        public MediaCoversUpdatedEvent(Book book, bool updated)
        {
            Book = book;
            Updated = updated;
        }
    }
}
