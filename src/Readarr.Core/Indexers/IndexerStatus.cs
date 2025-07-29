using Readarr.Core.Parser.Model;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.Indexers
{
    public class IndexerStatus : ProviderStatusBase
    {
        public ReleaseInfo LastRssSyncReleaseInfo { get; set; }
    }
}
