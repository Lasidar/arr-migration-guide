namespace Readarr.Core.ImportLists
{
    public interface IImportListRequestGenerator
    {
        ImportListPageableRequestChain GetListItems();
    }
}
