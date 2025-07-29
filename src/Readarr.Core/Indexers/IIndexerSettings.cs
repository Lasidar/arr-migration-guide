using System.Collections.Generic;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.Indexers
{
    public interface IIndexerSettings : IProviderConfig
    {
        string BaseUrl { get; set; }

        IEnumerable<int> MultiLanguages { get; set; }

        IEnumerable<int> FailDownloads { get; set; }
    }
}
