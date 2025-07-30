namespace Readarr.Core.Download
{
    public interface IProvideImportItemService
    {
        DownloadClientItem ProvideImportItem(DownloadClientItem item, DownloadClientItem previousImportAttempt, int downloadClientId);
    }

    public class ProvideImportItemService : IProvideImportItemService
    {
        private readonly IProvideDownloadClient _downloadClientProvider;

        public ProvideImportItemService(IProvideDownloadClient downloadClientProvider)
        {
            _downloadClientProvider = downloadClientProvider;
        }

        public DownloadClientItem ProvideImportItem(DownloadClientItem item, DownloadClientItem previousImportAttempt, int downloadClientId)
        {
            var client = _downloadClientProvider.Get(downloadClientId);

            return client.GetImportItem(item, previousImportAttempt);
        }
    }
}
