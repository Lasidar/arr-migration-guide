using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public interface IBookMetadataRepository : IBasicRepository<BookMetadata>
    {
        BookMetadata FindByForeignBookId(string foreignBookId);
        BookMetadata FindByGoodreadsId(string goodreadsId);
        BookMetadata FindByIsbn(string isbn);
        List<BookMetadata> FindByTitle(string title);
    }

    public class BookMetadataRepository : BasicRepository<BookMetadata>, IBookMetadataRepository
    {
        public BookMetadataRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public BookMetadata FindByForeignBookId(string foreignBookId)
        {
            return Query(b => b.ForeignBookId == foreignBookId).SingleOrDefault();
        }

        public BookMetadata FindByGoodreadsId(string goodreadsId)
        {
            return Query(b => b.GoodreadsId == goodreadsId).SingleOrDefault();
        }

        public BookMetadata FindByIsbn(string isbn)
        {
            return Query(b => b.Isbn == isbn || b.Isbn13 == isbn).SingleOrDefault();
        }

        public List<BookMetadata> FindByTitle(string title)
        {
            var cleanTitle = title.ToLowerInvariant();
            return Query(b => b.Title.ToLower() == cleanTitle || b.SortTitle.ToLower() == cleanTitle).ToList();
        }
    }
}