using Readarr.Core.Download;
using Readarr.Core.Parser.Model;

namespace Readarr.Core.MediaFiles.BookImport.Aggregation.Aggregators
{
    public interface IAggregateLocalEpisode
    {
        int Order { get; }
        LocalEpisode Aggregate(LocalEpisode localEpisode, DownloadClientItem downloadClientItem);
    }
}
