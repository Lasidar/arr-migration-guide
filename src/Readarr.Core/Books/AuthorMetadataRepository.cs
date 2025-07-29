using System.Collections.Generic;
using System.Linq;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;

namespace Readarr.Core.Books
{
    public interface IAuthorMetadataRepository : IBasicRepository<AuthorMetadata>
    {
        AuthorMetadata FindByForeignAuthorId(string foreignAuthorId);
        AuthorMetadata FindByGoodreadsId(string goodreadsId);
        List<AuthorMetadata> FindByName(string name);
    }

    public class AuthorMetadataRepository : BasicRepository<AuthorMetadata>, IAuthorMetadataRepository
    {
        public AuthorMetadataRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public AuthorMetadata FindByForeignAuthorId(string foreignAuthorId)
        {
            return Query(a => a.ForeignAuthorId == foreignAuthorId).SingleOrDefault();
        }

        public AuthorMetadata FindByGoodreadsId(string goodreadsId)
        {
            return Query(a => a.GoodreadsId == goodreadsId).SingleOrDefault();
        }

        public List<AuthorMetadata> FindByName(string name)
        {
            var cleanName = name.ToLowerInvariant();
            return Query(a => a.Name.ToLower() == cleanName || a.SortName.ToLower() == cleanName).ToList();
        }
    }
}