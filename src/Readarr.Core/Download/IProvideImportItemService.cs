namespace Readarr.Core.Download
{
    public interface IProvideImportItemService
    {
        DownloadClientItem ProvideImportItem(DownloadClientItem item, DownloadClientItem previousImportAttempt, int downloadClientId);
    }
}