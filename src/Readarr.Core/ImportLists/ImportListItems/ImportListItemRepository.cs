using System.Collections.Generic;
using Readarr.Core.Datastore;
using Readarr.Core.Messaging.Events;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.ImportLists.ImportListItems
{
    public interface IImportListItemRepository : IBasicRepository<ImportListItemInfo>
    {
        List<ImportListItemInfo> GetAllForLists(List<int> listIds);
    }

    public class ImportListItemRepository : BasicRepository<ImportListItemInfo>, IImportListItemRepository
    {
        public ImportListItemRepository(IMainDatabase database, IEventAggregator eventAggregator)
            : base(database, eventAggregator)
        {
        }

        public List<ImportListItemInfo> GetAllForLists(List<int> listIds)
        {
            return Query(x => listIds.Contains(x.ImportListId));
        }
    }
}
