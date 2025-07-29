using System.Collections.Generic;

namespace Readarr.Core.Books
{
    public interface IEditionService
    {
        Edition GetEdition(int editionId);
        List<Edition> GetEditions(int bookId);
        List<Edition> GetAllEditions();
        Edition GetEditionByFileId(int fileId);
        Edition UpdateEdition(Edition edition);
        void SetMonitored(IEnumerable<int> ids, bool monitored);
    }
}