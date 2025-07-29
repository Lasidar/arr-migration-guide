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
        Edition FindByForeignEditionId(string foreignEditionId);
        Edition FindByIsbn(string isbn);
        List<Edition> GetEditionsByAuthor(int authorId);
        List<Edition> GetEditionsByBookIds(List<int> bookIds);
        List<Edition> EditionsWithFiles(int authorId);
        List<Edition> EditionsWithoutFiles(int authorId);
        void InsertMany(List<Edition> editions);
        void UpdateMany(List<Edition> editions);
        void DeleteMany(List<Edition> editions);
        void SetFileId(int editionId, int fileId);
        void ClearFileId(int editionId);
    }
}