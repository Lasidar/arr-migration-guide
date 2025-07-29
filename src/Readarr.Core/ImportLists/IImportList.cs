using System;
using Readarr.Core.ThingiProvider;

namespace Readarr.Core.ImportLists
{
    public interface IImportList : IProvider
    {
        ImportListType ListType { get; }
        TimeSpan MinRefreshInterval { get; }
        ImportListFetchResult Fetch();
    }
}
