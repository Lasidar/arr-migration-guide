using System;
using Readarr.Core.ThingiProvider.Status;

namespace Readarr.Core.ImportLists
{
    public class ImportListStatus : ProviderStatusBase
    {
        public DateTime? LastInfoSync { get; set; }
        public bool HasRemovedItemSinceLastClean { get; set; }
    }
}
