using System.Collections.Generic;
using System.Threading.Tasks;
using Readarr.Core.Indexers;
using Readarr.Core.Parser.Model;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Download
{
    public interface IDownloadClient : IProvider
    {
        DownloadProtocol Protocol { get; }
        Task<string> Download(RemoteEpisode remoteEpisode, IIndexer indexer);
        Task<string> Download(RemoteBook remoteBook, IIndexer indexer);
        IEnumerable<DownloadClientItem> GetItems();
        DownloadClientItem GetImportItem(DownloadClientItem item, DownloadClientItem previousImportAttempt);
        void RemoveItem(DownloadClientItem item, bool deleteData);
        DownloadClientInfo GetStatus();
        void MarkItemAsImported(DownloadClientItem downloadClientItem);
    }
}
